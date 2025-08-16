# Intruder-Tools v0.1

**What is this?**

It's a set of tools that aim to make your mapmaking using the [Intruder Map Maker tools](https://sharklootgilt.superbossgames.com/wiki/index.php/IntruderMM) easier!

# What does it includes?

Right now:


 **1. Draw ZipLine in Editor**
 
Tired of trying to imagine what your zipline would look like? Imagine no more! 
Just add it to your zipline object and this preview will update in real time!

![Zipline script on zipline](https://github.com/goibacache/Intruder-Tools/blob/main/Release/imgs/ZipLine.png)

 **2. Disable broken CSGs**
	 
Use this on your CSG parent and it will detect and deactivate all of your CSGs that are stopping your baking!
Just select your Parent CSG and use the menu option!

![Deactivate option](https://github.com/goibacache/Intruder-Tools/blob/main/Release/imgs/Deactivate.png)

 **3. Recollide**

Using blender and tired of having to manually re-add colliders to your objects by hand because they don't update? Just select their parent and this tool will set all the children as static & add colliders automaticly! If you have objects that you don't want to have collision then just be sure that their name starts with "nc_" (as in "no collide").

![Recollide option](https://github.com/goibacache/Intruder-Tools/blob/main/Release/imgs/Recollide.png)
	 


# How to install?


Just grab this [UnityPackage](https://github.com/goibacache/Intruder-Tools/releases) from the releases tab (click the 3 dots on the top right and hit "download") and add it to your project! You'll get a new menu like this!

![New menu](https://github.com/goibacache/Intruder-Tools/blob/main/Release/imgs/Menu.png)

# Credits?
*Thanks to Zapan15 in the [unity forums](https://forum.unity.com/threads/progressive-gpu-error-failed-to-add-geometry-for-mesh-stud-mesh-is-missing-required-attribute-s.976230/#post-7092433) for giving me the idea to iterate through all of the CSGs*

 *Thanks to MitchyD for lending me his broken CSG to have a good way to test the script out!*

