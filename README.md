# SewingTable_TS3

# What is this?
This is the sewing table's code for TS3's Sewing table made by Greenplumbboblover/Lyralei. A more expanded conversion from TS2's Sewing table > TS3. (See: https://modthesims.info/d/643837/mechanical-stitching-machine-by-antique-artifacts-fully-functional-beta-v1-0-3.html )

# How do I contribute?
This Repository comes with all the DLL's and any necessities for it to work in Visual studio or SharpDevelop. This current project has been made in SharpDevelop so far, so it might mean that for Visual studio 2013 and up, you need to redo the settings: https://modthesims.info/wiki.php?title=Sims_3:Creating_a_game_compatible_Visual_Studio_project )

Any feature requests/Bugs can be found under 'Issues'. From those issues you may create a pull request in order to add/fix said issue/feature request. These, however are always checked. 

When working on a fix/addition, make sure to create a new branch called 'userName-Feature-issueName' or 'userName-bugfix-issueName'. Do NEVER commit directly to the master. This is just so that if something goes wrong, you can easily revert it back :) See: https://guides.github.com/ for any guidance.

Pull requests will always be checked by me :)

# Setup
So far, the project should be set up fine. However, there are a few things to keep in mind:
- We have 2 different C# solutions inside one whole project. One is the 'SewingTable', the other 'GlobalSewingTables' (also refered to as 'GlobalOptionsSewingTable'). GlobalSewingTables refers to what needs to be handled by the game on load/pre-load and globally. The 'SewingTable' project refers to what the object itself needs to do. 
- Because the "SewingTable' project is dependant on the code for 'GlobalSewingTables', every change we make to 'GlobalSewingTable' we need to refresh the reference dll in the 'Sewingtable' project. 
- Because of the pathing of each project is different, it might mean you'll need to re-add the 'GlobalSewingtable' dll in the references.
