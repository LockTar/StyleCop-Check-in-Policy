﻿//--------------------------------------------------------------------------
// <copyright file="SourceAnalysisPolicy.cs" company="Ralph Jansen">
//      Copyright (c) Ralph Jansen. All rights reserved.
//
//      The use and distribution terms for this software is covered by the
//      Microsoft Public License (Ms-PL) which can be found in the License.rtf 
//      at the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by
//      the terms of this license.
//
//      You must not remove this notice, or any other, from this software.
// </copyright>
//--------------------------------------------------------------------------

namespace RalphJansen.StyleCopCheckInPolicy
{
	using System;
	using System.Collections.ObjectModel;
	using System.Windows.Forms;
	using EnvDTE;
	using Microsoft.TeamFoundation.VersionControl.Client;
	using Microsoft.VisualStudio.Shell;
	using StyleCop;
	using RalphJansen.StyleCopCheckInPolicy.Policy;
	using RalphJansen.StyleCopCheckInPolicy.Policy.Exclusions;
	using RalphJansen.StyleCopCheckInPolicy.Properties;
	using RalphJansen.StyleCopCheckInPolicy.UI.Forms;
	using RalphJansen.StyleCopCheckInPolicy.VisualStudio;

	/// <summary>
	/// Provides a check-in policy for Microsoft Team Foundation Server to check for source analysis violations with Microsoft StyleCop. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class SourceAnalysisPolicy : PolicyBase
	{
		#region Fields

		/// <summary>
		/// Contains the provider used to manage error tasks within Visual Studio.
		/// </summary>
		[NonSerialized]
		private static ViolationTaskProvider taskProvider;

		/// <summary>
		/// Contains the settings for the policy.
		/// </summary>
		private PolicySettings settings;

		/// <summary>
		/// The Visual Studio Shell.
		/// </summary>
		private DTE dte;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the <see cref="RalphJansen.StyleCopCheckInPolicy.SourceAnalysisPolicy"/> class.
		/// </summary>
		public SourceAnalysisPolicy()
		{
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value indicating whether the policy can be edited.
		/// </summary>
		public override bool CanEdit
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the description of the policy.
		/// </summary>
		public override string Description
		{
			get { return Resources.Message_PolicyDescription; }
		}

		/// <summary>
		/// Gets the type of policy.
		/// </summary>
		public override string Type
		{
			get { return Resources.Message_PolicyType; }
		}

		/// <summary>
		/// Gets the policy type description.
		/// </summary>
		public override string TypeDescription
		{
			get { return Resources.Message_PolicyTypeDescription; }
		}

		/// <summary>
		/// Gets or sets the policy settings.
		/// </summary>
		private PolicySettings Settings
		{
			get
			{
				if (this.settings == null)
				{
					this.settings = PolicySettings.Create(true);
				}

				return this.settings;
			}

			set
			{
				this.settings = value;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// Activates a policy failure.
		/// </summary>
		/// <param name="failure">The failure to activate.</param>
		public override void Activate(PolicyFailure failure)
		{
			SourceAnalysisPolicyFailure policyFailure = failure as SourceAnalysisPolicyFailure;
			if (policyFailure != null)
			{
				// Create dialog to show the violations
				using (DisplayViolationsDialog dialog = new DisplayViolationsDialog())
				{
					// Set the violations in the dialog.
					dialog.Violations = policyFailure.Violations;

					// Show dialog
					dialog.ShowDialog();

					// Check if there is a selected violation
					if (dialog.SelectedViolation != null)
					{
						// There is a selected violation so move to it.
						ViolationTask.MoveToViolation(dte, dialog.SelectedViolation);
					}
				}
			}

			base.Activate(failure);
		}
		
		/// <summary>
		/// Show dialog for editing the settings of the checkin policy.
		/// </summary>
		/// <param name="policyEditArgs">An <see cref="IPolicyEditArgs"/> containing policy edit arguments.</param>
		/// <returns><b>true</b> if the policy has been edited successfully, otherwise <b>false</b>.</returns>
		public override bool Edit(IPolicyEditArgs policyEditArgs)
		{
			if (policyEditArgs == null)
			{
				ThrowHelper.ThrowArgumentNullException("policyEditArgs");
			}

			bool retval = false;

			using (EditPolicyDialog dialog = new EditPolicyDialog())
			{
				// Clone the settings to prevent modifying the settings currently in use.
				dialog.Settings = (PolicySettings)this.Settings.Clone();

				if (dialog.ShowDialog(policyEditArgs.Parent) == DialogResult.OK)
				{
					this.Settings = dialog.Settings;
					retval = true;
				}
			}

			return retval;
		}

		/// <summary>
		/// Evaluates the policy and set the violations in the task list (error window of Visual Studio).
		/// </summary>
		/// <returns>The policy failures, if any, that occurred.</returns>
		public override PolicyFailure[] Evaluate()
		{
			PolicyFailure[] failures = null;

			using (EvaluationProcess process = new EvaluationProcess())
			{
				// Analyse if there are any violations
				process.Initialize(new EvaluationContext(this, this.Settings, this.PendingCheckin));
				failures = process.Analyze();

				if (taskProvider != null)
				{
					// Set the settings into the taskprovider and clear it.
					taskProvider.Settings = this.Settings;
					taskProvider.Clear();

					// If there are violations, add them to the tasks list (error window)
					if (failures != null && failures.Length > 0)
					{
						foreach (PolicyFailure failure in failures)
						{
							SourceAnalysisPolicyFailure policyFailure = (SourceAnalysisPolicyFailure)failure;

							foreach (Violation violation in policyFailure.Violations)
							{
								taskProvider.AddTask(violation);
							}
						}
					}
				}
			}

			return failures;
		}
		
		/// <summary>
		/// Initializes the policy.
		/// </summary>
		/// <param name="pendingCheckin">The <see cref="IPendingCheckin"/> to use during initialization.</param>
		public override void Initialize(IPendingCheckin pendingCheckin)
		{
			if (pendingCheckin == null)
			{
				ThrowHelper.ThrowArgumentNullException("pendingCheckin");
			}

			// Get the reference to the VS ide.
			dte = (DTE)pendingCheckin.GetService(typeof(DTE));

			if (taskProvider != null)
			{
				// Initialize zo clear the taskprovider.
				taskProvider.Clear();
			}
			else
			{
				if (dte != null && dte.Application != null)
				{
					taskProvider = new ViolationTaskProvider(
						new ServiceProvider((Microsoft.VisualStudio.OLE.Interop.IServiceProvider)dte.Application),
						this.Settings);
				}
			}

			base.Initialize(pendingCheckin);
		}

		/// <summary>
		/// Disposes of the policy.
		/// </summary>
		public sealed override void Dispose()
		{
			this.settings = null;

			base.Dispose();
		}
		
		#endregion
	}
}