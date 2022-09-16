C#通过WMI来获取系统或设备的信息：

************ Win32_BaseBoard ***************
1. Caption = 基板
2. ConfigOptions = System.String[]
3. CreationClassName = Win32_BaseBoard
4. Depth = 
5. Description = 基板
6. Height = 
7. HostingBoard = True
8. HotSwappable = False
9. InstallDate = 
10. Manufacturer = ASUSTeK COMPUTER INC.
11. Model = 
12. Name = 基板
13. OtherIdentifyingInfo = 
14. PartNumber = 
15. PoweredOn = True
16. Product = M7600QE   // 可以获取正确的 Product 值
17. Removable = False
18. Replaceable = True
19. RequirementsDescription = 
20. RequiresDaughterBoard = False
21. SerialNumber = M831NBCX001MH0MB  // SN 不正确，可通过cmd命令行来获取正确的SN进行比较
22. SKU = 
23. SlotLayout = 
24. SpecialRequirements = 
25. Status = OK
26. Tag = Base Board
27. Version = 1.0       
28. Weight = 
29. Width = 
**********************************************

************ Win32_Bios ***************
1. BiosCharacteristics = System.UInt16[]
2. BIOSVersion = System.String[]
3. BuildNumber = 
4. Caption = M7600QE.303
5. CodeSet = 
6. CurrentLanguage = en|US|iso8859-1
7. Description = M7600QE.303
8. EmbeddedControllerMajorVersion = 255
9. EmbeddedControllerMinorVersion = 255
10. IdentificationCode = 
11. InstallableLanguages = 1
12. InstallDate = 
13. LanguageEdition = 
14. ListOfLanguages = System.String[]
15. Manufacturer = American Megatrends International, LLC.
16. Name = M7600QE.303
17. OtherTargetOS = 
18. PrimaryBIOS = True
19. ReleaseDate = 20210805000000.000000+000
20. SerialNumber = M8NTCX000173310       // 获取正确的SN号
21. SMBIOSBIOSVersion = M7600QE.303
22. SMBIOSMajorVersion = 3
23. SMBIOSMinorVersion = 3
24. SMBIOSPresent = True
25. SoftwareElementID = M7600QE.303
26. SoftwareElementState = 3
27. Status = OK
28. SystemBiosMajorVersion = 5
29. SystemBiosMinorVersion = 19
30. TargetOperatingSystem = 0
31. Version = _ASUS_ - 1072009
**********************************************
























































