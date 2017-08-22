# StyleCop-Check-in-Policy

In progress... no time indication yet to finish.

Working on update for Visual Studio 2017 with latest StyleCop rules.



This GitHub site is the sourcecode for the Visual Studio check-in policy for checking your code with StyleCop. This check-in policy could be installed via the [Marketplace](https://marketplace.visualstudio.com) for [Visual Studio 2013](https://marketplace.visualstudio.com/items?itemName=RalphJansen.StyleCopCheck-inPolicy2013), [Visual Studio 2015](https://marketplace.visualstudio.com/items?itemName=RalphJansen.StyleCopCheck-inPolicy2015).


## Functions
* Global StyleCop settings for whole Team Project
* Import your current StyleCop settings (if you already use StyleCop) into the check-in policy
* Export the StyleCop settings of the check-in policy
* Override the settings for specific projects with the power of merged StyleCop settings files
* Thread the Check-in violations as errors, warnings or information messages in Visual Studio
* Exclude folders to be checked

## Requirements
* Visual Studio 2013
* Visual Studio 2015
* Visual Studio 2017 (Coming soon)

*Optional note*: 
Set the `/p:StyleCopTreatErrorsAsWarnings=false` parameter in your build definition to throw an error when there is a StyleCop warning. This works if you have the NuGet [StyleCop.MSBuild](https://www.nuget.org/packages/StyleCop.MSBuild/) package installed. This way your build will fail so no corrupt code is checked-in. This check-in policy prevents users from checking-in bad code but it is a check-in policy so it could be overridden. You can set this parameter in your build definition on the "Process" tab, under the "Advanced" category and then the "MSBuild Arguments" property.

