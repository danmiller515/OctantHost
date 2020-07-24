# OctantHost
This is a basic windows GUI for Octant.

I have been looking for a decent GUI for managing our Kubernetes clusters and Octant for me was perfect. The only drawback is it's a web site run from you machines console. 

This lead me to create this application. It started off as a tray icon that ran the Octant application and later I added a web browser using CefSharp to make it feel a little more native.

I understand that hosting the application in electron is there but has issues on windows. So far this has been my workaround. 

To build it you will need to get the latest version of Octant and copy it into the octant folder.
