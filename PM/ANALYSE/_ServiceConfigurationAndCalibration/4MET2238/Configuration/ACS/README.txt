"BufferACS2238.PRG" is a copy of the ACS buffers.
If a modification is made in any of the buffers, it is essential to version them in git.
To do this, make a copy after flashing the controller.

procedure available on the wiki Metro with screen capture.

To write into a buffer : 
1. Start SPiiPlus MMI Application Studio.
	Menu Start > SPiiPlus ADK Suite v2.50 > SPiiPlus MMI Application Studio 2.50
2.  Click on Add Controller
3. Open the General tab.
	Open the Ethernet sub-tab.
	Enter the Controller IP Address : 20.20.249.41
	Enter the Controller Port : 701
	Click on Connect.
4. The controller should appear.
	The actual name may vary depending on the tool.
5. Select the controller (the actual name depends on the tool).
	Right-click on the controller --> Add Component > Application Development > Application Wizard
6. . Check Save Application to PC.
	Click on Next.
7. . Fill User, Application, Remarks.
	Check the SP Programs.
	Click on Save.
	(note : the "Save to Flash" seems to be a buggy popup name).
8. Fill File, User, Machine, etc.
	Click on Next.
	
	
Buffer 1 :
Axes, safety, SLLIMIT, SRLIMIT, input, output

Buffer 2:
Safety and errorhandling
System Fauls for all axis

Buffer 3 :
Initialization of Process functions

Buffer 4 :
Initialization of Gantry Stage

Buffer 5 :
?

Buffer 6 :
?

Buffer 7 : 
?

Buffer 10 :
Global variable

