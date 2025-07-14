#define PGLVK_LBUTTON        0x01
#define PGLVK_RBUTTON        0x02
#define PGLVK_CANCEL         0x03
#define PGLVK_MBUTTON        0x04

#define PGLVK_BACK           0x08
#define PGLVK_TAB            0x09

#define PGLVK_CLEAR          0x0C
#define PGLVK_RETURN         0x0D

#define PGLVK_SHIFT          0x10
#define PGLVK_CONTROL        0x11
#define PGLVK_MENU           0x12
#define PGLVK_PAUSE          0x13
#define PGLVK_CAPITAL        0x14


#define PGLVK_ESCAPE         0x1B

#define PGLVK_SPACE          0x20
#define PGLVK_PRIOR          0x21
#define PGLVK_NEXT           0x22
#define PGLVK_END            0x23
#define PGLVK_HOME           0x24
#define PGLVK_LEFT           0x25
#define PGLVK_UP             0x26
#define PGLVK_RIGHT          0x27
#define PGLVK_DOWN           0x28
#define PGLVK_SELECT         0x29
#define PGLVK_PRINT          0x2A
#define PGLVK_EXECUTE        0x2B
#define PGLVK_SNAPSHOT       0x2C
#define PGLVK_INSERT         0x2D
#define PGLVK_DELETE         0x2E
#define PGLVK_HELP           0x2F

#define PGLVK_0				 0x30
#define PGLVK_1				 0x31
#define PGLVK_2				 0x32
#define PGLVK_3				 0x33
#define PGLVK_4				 0x34
#define PGLVK_5				 0x35
#define PGLVK_6				 0x36
#define PGLVK_7				 0x37
#define PGLVK_8				 0x38
#define PGLVK_9				 0x39

#define PGLVK_A				 0x41
#define PGLVK_B				 0x42
#define PGLVK_C				 0x43
#define PGLVK_D				 0x44
#define PGLVK_E				 0x45
#define PGLVK_F				 0x46
#define PGLVK_G				 0x47
#define PGLVK_H				 0x48
#define PGLVK_I				 0x49
#define PGLVK_J				 0x4A
#define PGLVK_K				 0x4B
#define PGLVK_L				 0x4C
#define PGLVK_M				 0x4D
#define PGLVK_N				 0x4E
#define PGLVK_O				 0x4F
#define PGLVK_P				 0x50
#define PGLVK_Q				 0x51
#define PGLVK_R				 0x52
#define PGLVK_S				 0x53
#define PGLVK_T				 0x54
#define PGLVK_U				 0x55
#define PGLVK_V				 0x56
#define PGLVK_W				 0x57
#define PGLVK_X				 0x58
#define PGLVK_Y				 0x59
#define PGLVK_Z				 0x5A

#define PGLVK_LWIN           0x5B
#define PGLVK_RWIN           0x5C
#define PGLVK_APPS           0x5D

#define PGLVK_NUMPAD0        0x60
#define PGLVK_NUMPAD1        0x61
#define PGLVK_NUMPAD2        0x62
#define PGLVK_NUMPAD3        0x63
#define PGLVK_NUMPAD4        0x64
#define PGLVK_NUMPAD5        0x65
#define PGLVK_NUMPAD6        0x66
#define PGLVK_NUMPAD7        0x67
#define PGLVK_NUMPAD8        0x68
#define PGLVK_NUMPAD9        0x69
#define PGLVK_MULTIPLY       0x6A
#define PGLVK_ADD            0x6B
#define PGLVK_SEPARATOR      0x6C
#define PGLVK_SUBTRACT       0x6D
#define PGLVK_DECIMAL        0x6E
#define PGLVK_DIVIDE         0x6F
#define PGLVK_F1             0x70
#define PGLVK_F2             0x71
#define PGLVK_F3             0x72
#define PGLVK_F4             0x73
#define PGLVK_F5             0x74
#define PGLVK_F6             0x75
#define PGLVK_F7             0x76
#define PGLVK_F8             0x77
#define PGLVK_F9             0x78
#define PGLVK_F10            0x79
#define PGLVK_F11            0x7A
#define PGLVK_F12            0x7B
#define PGLVK_F13            0x7C
#define PGLVK_F14            0x7D
#define PGLVK_F15            0x7E
#define PGLVK_F16            0x7F
#define PGLVK_F17            0x80
#define PGLVK_F18            0x81
#define PGLVK_F19            0x82
#define PGLVK_F20            0x83
#define PGLVK_F21            0x84
#define PGLVK_F22            0x85
#define PGLVK_F23            0x86
#define PGLVK_F24            0x87

#define PGLVK_NUMLOCK        0x90
#define PGLVK_SCROLL         0x91

#define PGLVK_LSHIFT         0xA0
#define PGLVK_RSHIFT         0xA1
#define PGLVK_LCONTROL       0xA2
#define PGLVK_RCONTROL       0xA3
#define PGLVK_LMENU          0xA4
#define PGLVK_RMENU          0xA5

#define PGLVK_ATTN           0xF6
#define PGLVK_CRSEL          0xF7
#define PGLVK_EXSEL          0xF8
#define PGLVK_EREOF          0xF9
#define PGLVK_PLAY           0xFA
#define PGLVK_ZOOM           0xFB
#define PGLVK_NONAME         0xFC
#define PGLVK_PA1            0xFD
#define PGLVK_OEM_CLEAR      0xFE

void PGL_CONV pglUpdateKeyboardState(PGLCommon*);
ubool PGL_CONV pglUpdateJoystickState(PGLCommon*);
void PGL_CONV pglUpdateMouseState(PGLCommon*);
