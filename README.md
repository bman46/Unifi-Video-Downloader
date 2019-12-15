# Unifi Video Downloader
[![Build status](https://ci.appveyor.com/api/projects/status/sgpp8a63vbm4jod3?svg=true)](https://ci.appveyor.com/project/bman46/unifi-video-downloader)

Downloads video files from Unifi Video and compiles then into a single video.
## Usage:
Requires the following arguments in this order, seperated by a space:
  1. IP
  2. Username
  3. Password
  4. SSH Fingerprint (Similar to this: ssh-ed25519 256 xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx and can be fould by connecting to the client in WinSCP)
  5. Path to recording folder (Similar to: /mnt/raid0/recordings/)
  
I reccomand making a shortcut to this application with the parameters inclueded.
Make sure to remove the video files from the %TEMP% folder when done using.
###### Note: This does not use the unifi username and password, rather the one to the linux system. requires sudo without password (I use a seperate account for UFV and i dont give non-root users permission to copy or modify the files).
