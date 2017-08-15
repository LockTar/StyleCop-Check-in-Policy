# StyleCop-Check-in-Policy

In progress... no time indication yet to finish.

Working on update for Visual Studio 2017 with latest StyleCop rules.



This GitHub site is the sourcecode for the Visual Studio check-in policy for checking your code with StyleCop. This check-in policy could be installed by the [Visual Studio gallery](https://visualstudiogallery.msdn.microsoft.com) for [Visual Studio 2013](http://visualstudiogallery.msdn.microsoft.com/5fb39f7a-3199-4bbd-94ca-0aaad00a94e6), [Visual Studio 2015](https://visualstudiogallery.msdn.microsoft.com/e950c06d-774c-4f3e-bc2c-0ea4784f1a3c) or you can download it from the [download page](https://stylecopcheckinpolicy.codeplex.com/releases).


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


*Disclaimer note*: 
The original code is from another CodePlex project called [Source Analysis Policy](https://sourceanalysispolicy.codeplex.com). This code is altered because the original project stopped supporting new versions of Visual Studio and StyleCop. The functionality is for some parts the same but a lot is altered like support for the latest StyleCop version and the new extension methods of Visual Studio with vsix files.
