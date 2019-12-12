# Unifi Video Downloader
Downloads video files for Unifi Video and compiles then into a single video
## Usage:
Requires the following command line arguments in this order, seperated by a space:
  1. IP
  2. Username
  3. Password
  4. SSH Fingerprint (Similar to this: ssh-ed25519 256 xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx)
  5. Path to recording folder (Similar to: /mnt/raid0/recordings/)
  
Make sure to remove the video files from the %TEMP% file when done.
###### Note: This is not the unifi username and password, rather the one to the linux system. requires sudo without password (I use a seperate account for UFV and i dont have permission to copy on non root users).
