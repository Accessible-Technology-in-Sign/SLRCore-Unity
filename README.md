# Acknowledgements
Unity toolkit for Sign Language Recognition Integration. 

Designed and Maintained by
- Ananay Gupta (ananay@gatech.edu)
- Nicholas Unger (nunger3@gatech.edu)
- And the Unity subteam members:
- Marie Andry
- Charlie Lin

# Setup

Getting started with the toolkit is relatively simple, but there are a few things you need to do to make sure you're ready to go.

## Step 1: Download Unity Hub.

- Assuming you do not already have a version of Unity to work with, you'll need to install Unity and the version of Unity the toolkit is built on. Navigate to [Unity](https://unity.com/download) and download Unity Hub for your platform of choice.
  
<img width="1471" alt="Screenshot 2024-09-24 at 3 19 33 PM" src="https://github.com/user-attachments/assets/6c4e6845-ce08-42de-9107-d4b2a9222c1a">

- Navgiate to the [Unity download archive.](https://unity.com/download)
  
<img width="1357" alt="Screenshot 2024-09-24 at 3 28 04 PM" src="https://github.com/user-attachments/assets/79566cf6-735d-49a3-ab27-ea42d9f2c204">

- Search for Version 2022.3.45f1. Select "Hub installation".
  
<img width="1789" alt="Screenshot 2024-09-24 at 3 29 36 PM" src="https://github.com/user-attachments/assets/c1c54a45-116b-4ca1-b7e9-e134774e4875">

- Within Unity Hub, install Version 2022.3.45f1. Select the platforms you anticipate building your game to in the future. (The screenshots are not comprehensive; review all the platforms you can build to in the installation wizard and select the ones you need.)

<img width="880" alt="Screenshot 2024-09-24 at 5 06 45 PM" src="https://github.com/user-attachments/assets/22537268-2f5e-40ad-8933-2afd5c13e279">

<img width="879" alt="Screenshot 2024-09-24 at 5 12 59 PM" src="https://github.com/user-attachments/assets/205c5a3d-54fc-43a9-a4ca-1f09a14793da">


## Step 2: Clone the repo and add the project.

- This is pretty simple. Clone this repo using either [GitHub Desktop](https://github.com/apps/desktop) or the Terminal or Command Line.
- In Unity Hub, select Add -> Add Project from Disk and navigate to where you cloned the SLRCore-Unity repo to. You can now use the toolkit locally.
- Note: If you try to open the project before adding the necessary packages, you may receive a warning asking you if you want to enter Safe Mode. Click ignore for now. These compilation errors will go away once you download the necessary packages.

## Step 3: Download the necessary packages and add them to the project.

### Step 3a: Download the MediaPipe for Unity plugin
- Navigate to Homuler's [MediaPipe plugin for Unity](https://github.com/homuler/MediaPipeUnityPlugin/tree/master?tab=readme-ov-file) and then find the [releases page.](https://github.com/homuler/MediaPipeUnityPlugin/releases)
- From the most recent release, download the file with the file format .unitypackage. In the attached screenshot, it is named "MediaPipe.0.15.0.unitypackage".
  
<img width="913" alt="Screenshot 2024-09-25 at 11 45 31 PM" src="https://github.com/user-attachments/assets/4a886576-c25c-4c3a-8554-0137d33c6cb7">

- Now you can add the package to the toolkit. Open the SLRCore-Unity project in Unity Hub, and from the menu bar, select Assets -> Import Package -> Custom Package...

<img width="1426" alt="Screenshot 2024-09-26 at 12 13 46 AM" src="https://github.com/user-attachments/assets/e4793373-37e3-46e1-92a4-a394b109f3ee">

- In your OS's file explorer, navigate to where you downloaded the MediaPipe for Unity package and select it.
- In the import Unity Package Wizard, you can select what to import and what to not import. If you're unsure, import everything.
<img width="1016" alt="Screenshot 2024-09-26 at 12 26 33 AM" src="https://github.com/user-attachments/assets/a28ab1b4-e0ca-41d9-8764-2a196cdb792c">

- You've successfully added the MediaPipe plugin.

### Step 3b: Install TensorFlow Lite for Unity.

- From the Unity project, select Edit -> Project Settings in the menu bar.
- A pop-up window should open. Navigate to the Package Manager tab.

<img width="818" alt="Screenshot 2024-09-26 at 12 43 07 AM" src="https://github.com/user-attachments/assets/ac01a727-71a6-4161-b71a-aa19f8e323dd">

- Click the plus button in the bottom left of the left column of the "Scoped Registries" tab to add a new scoped registry.

<img width="825" alt="Screenshot 2024-09-26 at 12 46 55 AM" src="https://github.com/user-attachments/assets/ce364449-4d8b-42ac-b237-242a7669442e">

- You now need to add two scoped registries. Fill in the Name, URL, and Scope(s) exactly as shown below.

<img width="823" alt="Screenshot 2024-09-26 at 12 56 29 AM" src="https://github.com/user-attachments/assets/01daa7fb-0cfc-43a8-8302-08b8fd9d6b2c">
<img width="822" alt="Screenshot 2024-09-26 at 12 56 38 AM" src="https://github.com/user-attachments/assets/3cd45ef0-caa3-4ef0-a2a4-77ca169abb1c">

- Close the Project Settings window. In the sidebar, click the packages folder, then, in the menu bar, select Assets -> View in Package Manager.

<img width="1427" alt="Screenshot 2024-09-26 at 1 43 29 AM" src="https://github.com/user-attachments/assets/ce540003-58a7-4c29-9784-15a39911c165">

- In the Package Manager pop up window, select the drop-down and select "My Registries".

<img width="794" alt="Screenshot 2024-09-26 at 1 44 23 AM" src="https://github.com/user-attachments/assets/0c507928-4185-4213-982e-a3dc8b3da6c8">

- Under the dropdown labeled "Koki Ibukuro," find "TensorFlow Lite" and click the Install button.

<img width="799" alt="Screenshot 2024-09-26 at 1 46 15 AM" src="https://github.com/user-attachments/assets/e676edce-77fb-42a5-830b-b5c7b5e5c876">

- Assuming you've also added the MediaPipe plugin package, the toolkit is now fully operational and you can use the wealth of hand recognition features to create sign language recognition games for multiple platforms.

## Step 4: Build your game.

- Once you're done developing your game, you can build to your platform of choice. From the menu bar, select File -> Build Settings if you need to change aspects of your build.

<img width="641" alt="Screenshot 2024-09-26 at 1 49 16 AM" src="https://github.com/user-attachments/assets/cf68e762-e31d-43b9-8cac-433bf0cf30b9">

- The Build Settings window should pop up. From here, you can select which scenes you would like to build as well as which platform you would like to build to. Keep in mind that, if you are changing build platforms (say, from Windows to Android,) you must select the "Switch Platform" button and allow Unity some time to prepare to build for that platform. Each platform you build for has its own platform specific settings in addition to the build specific settings. For more information, refer to the [Unity documentation on build settings.](https://docs.unity3d.com/Manual/BuildSettings.html)

## Step 5: Happy developing!

- That's all you need to get started with the Unity Sign Language Recognition toolkit. The toolkit gives you access to robust tools to help quickly develop games that take advantage of sign language recognition capability and create experiences unlike anything seen before.

- If you have questions, please feel free to reach out to Ananay Gupta or Nicholas Unger, whose emails are listed above.

