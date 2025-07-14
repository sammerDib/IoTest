
#ifdef SPG_General_USETREEVIEW

typedef struct
{
	void* Element;
	BYTE User[];
} SPG_TreeElement;

typedef struct
{
	int MaxElements;
	int NumElements;
	int UserSize;
	BYTE* E;
} SPG_Tree;

#define SPG_TreeElement(T,i) (((SPG_TreeElement*)(T.E+(i)*(sizeof(SPG_TreeElement)+T.UserSize)))->Element)
#define SPG_TreeUser(T,i) (((SPG_TreeElement*)(T.E+(i)*(sizeof(SPG_TreeElement)+T.UserSize)))->User)

int SPG_CONV SPG_TreeInit(SPG_Tree& T, int MaxElements, int UserSize);
void SPG_CONV SPG_TreeClose(SPG_Tree& T);
int SPG_CONV SPG_TreeAdd(SPG_Tree& T, void* Element, void* User);//element est pointé, user est recopie en local

#endif

