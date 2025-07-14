
#ifndef LPCSTR
#define LPCSTR char*
#endif

class flipDialogTemplate
{
public:
    flipDialogTemplate(LPCSTR caption, DWORD style, int x, int y, int w, int h,
        LPCSTR font = 0, WORD fontSize = 8);
    void AddComponent(LPCSTR type, LPCSTR caption, DWORD style, DWORD exStyle,
        int x, int y, int w, int h, WORD id);
    void AddButton(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
        int w, int h, WORD id);
    void AddEditBox(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
        int w, int h, WORD id);
    void AddStatic(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
        int w, int h, WORD id);
    void AddListBox(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
        int w, int h, WORD id);
    void AddScrollBar(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
        int w, int h, WORD id);
    void AddComboBox(LPCSTR caption, DWORD style, DWORD exStyle, int x, int y,
        int w, int h, WORD id);

    /**
     * Returns a pointer to the Win32 dialog template which the object
     * represents. This pointer may become invalid if additional
     * components are added to the template.
     */
    operator const DLGTEMPLATE*() const;
    virtual ~flipDialogTemplate();

protected:
    void AddStandardComponent(WORD type, LPCSTR caption, DWORD style,
        DWORD exStyle, int x, int y, int w, int h, WORD id);
    void AlignData(int size);
    void AppendString(LPCSTR string);
    void AppendData(void* data, int dataLength);
    void EnsureSpace(int length);
private:
    DLGTEMPLATE* dialogTemplate;
    int totalBufferLength;
    int usedBufferLength;
	
};
