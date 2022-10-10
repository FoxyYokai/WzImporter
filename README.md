
Tool for importing equipment/face/hair from later versions of maple into v83

----

Instructions:

This is a very memory intensive program, use at your own risk.
This has been tested only with importing TO v83. It assumes you are importing to an older version (GMS old) from a newer version (BMS/GMS/MSEA)
You will need:\
The Character.wz you want to import to.\
The Character.wz you want to import from.\
The String.wz you want to import to (if you select the include String.wz option)\
The String.wz you want to import from (if you select the include String.wz option)

The program does not modify your original Character.wz. It creates a copy in the output folder.

This program does not adjust or modify effects that don't work in v83 normally, so some items with effects may not display as they do in later versions due to incompatibility.\
This program converts _inlink and _outlink properties from later versions to UOL properties with parsed paths. It also adjusts any UOL that reference other UOL properties, as this is not supported in v83.

If you encounter any bugs, please contact or open a pull request with fixes.

----

##### Third party libraries

##### MapleLib2 by haha01haha01;
 - based on MapleLib by Snow;
 - based on WzLib by JonyLeeson;
 - based on information from Fiel\Koolk.
