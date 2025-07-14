
/*

//##################          TREE UTILITY FUNCTIONS         ##############

typedef struct SPG_LINEAR_STORAGE
{
	int Size;
	SPG_LINEAR_STORAGE* Next;//next=0 pour le dernier
	//SPG_TREE_STORAGE* First;
	SPG_LINEAR_STORAGE* Last;
	char* NextElement;//in this storage
	char* EndElement;//in this storage
	char Elements[];
} SPG_LINEAR_STORAGE;

SPG_LINEAR_STORAGE* SPG_CONV LST_Init(int Size)
{
	SPG_LINEAR_STORAGE* S=(SPG_LINEAR_STORAGE*)SPG_MemAlloc(sizeof(SPG_LINEAR_STORAGE)+Size,"LST_Init");
	S->Size=Size;
	S->Next=0;
	//S->First=S;
	S->Last=S;
	S->NextElement=S->Elements;
	S->EndElement=S->Elements+S->Size;
	return S;
}

void SPG_CONV LST_Close(SPG_LINEAR_STORAGE* S)
{
	//S=S->First;
	for(SPG_LINEAR_STORAGE* C=S->Next;S;S=C,C=C?C->Next:0)
	{
		SPG_MemFree(S);
	}
	return;
}

void* SPG_CONV LST_Store(SPG_LINEAR_STORAGE* S, void* Element, int szElement) //returns stored element position
{
	SPG_LINEAR_STORAGE* L=S->Last;
	if(L->NextElement+szElement>L->EndElement)
	{//besoin d'une nvelle structure
		L->Next=LST_Init(2*V_Min(L->Size,szElement)); //la taille progresse par 2 a chaque iteration
		//L->Next->First=L->First;
		L=L->Next;
		for(SPG_LINEAR_STORAGE* U=S;U;U=U->Next) { U->Last=L; } //met a jour la reference vers Last de tous les elements existants
	}
	char* E=L->NextElement;
	L->NextElement+=szElement;
	if(Element) memcpy(E,Element,szElement);
	return E;
}

typedef struct SPG_TNODE
{
	SPG_TNODE* Parent;
	SPG_TNODE* Left;
	SPG_TNODE* Right;
	int szElement;
	char Data[];
} SPG_TNODE;

typedef struct
{
	SPG_LINEAR_STORAGE S;
} SPG_TREE;

SPG_TREE* SPG_CONV STREE_Init(int StartSize)
{
	SPG_TREE* t=(SPG_TREE*)LST_Init(StartSize+sizeof(SPG_TNODE));
	return t;
}

SPG_TNODE* SPG_CONV STREE_Root(SPG_TREE* t)
{
	CHECK(t==0,"STREE_Root",return 0);
	return (SPG_TNODE*)(t->S.Elements);
}

bool SPG_CONV STREE_IsRoot(SPG_TREE* t, SPG_TNODE* n) return n==(SPG_TNODE*)(t->S.Elements);

void SPG_CONV STREE_SetData(SPG_TREE* t, SPG_TNODE* n, void* Element, int szElement)
{
	CHECK(t==0,"STREE_SetData",return);
	CHECK(n==0,"STREE_SetData",return);
	CHECK(STREE_IsRoot(t,n),"STREE_SetData",return);//root n'accepte pas de donnees il existe seulement pour permettre l'insertion de noeud parent dans l'arbre
	CHECK(n->szElement!=szElement,"",return);
	if(Element) memcpy(w->Data,Element,szElement);
	return;
}

//up one directory : InsertRight(n->Parent)

SPG_TNODE* SPG_CONV STREE_PushLeft(SPG_TREE* t, SPG_TNODE* n, void* Element, int szElement) //add directory
{
	CHECK(t==0,"STREE_InsertLeft",return 0);
	CHECK(n==0,"STREE_InsertLeft",return 0);
	SPG_TNODE* w = (SPG_TNODE*)LST_Store(&(t->S),0,sizeof(SPG_TNODE)+szElement);
	w->Parent=n->Parent;
	n->Parent=w;
	w->Left=n;
	w->Right=0;
	w->szElement=szElement;
	STREE_SetData(t,w,Element,szElement);
	return w;
}

SPG_TNODE* SPG_CONV STREE_PushRight(SPG_TREE* t, SPG_TNODE* n, void* Element, int szElement) //create subdirectory
{
	CHECK(t==0,"STREE_InsertRight",return 0);
	CHECK(n==0,"STREE_InsertRight",return 0);
	SPG_TNODE* w = (SPG_TNODE*)LST_Store(&(t->S),0,sizeof(SPG_TNODE)+szElement);
	w->Parent=n->Parent;
	n->Parent=w;
	w->Right=n;
	w->Left=0;
	w->szElement=szElement;
	STREE_SetData(t,w,Element,szElement);
	return w;
}

SPG_TNODE* SPG_CONV STREE_InsertLeft(SPG_TREE* t, SPG_TNODE* n, void* Element, int szElement) //add directory
{
	CHECK(t==0,"STREE_InsertLeft",return 0);
	CHECK(n==0,"STREE_InsertLeft",return 0);
	SPG_TNODE* w = (SPG_TNODE*)LST_Store(&(t->S),0,sizeof(SPG_TNODE)+szElement);
	w->Parent=n;
	w->Left=n->Left;
	if(n->Left) n->Left->Parent=w;
	w->Right=0;
	w->szElement=szElement;
	STREE_SetData(t,w,Element,szElement);
	return w;
}

SPG_TNODE* SPG_CONV STREE_InsertRight(SPG_TREE* t, SPG_TNODE* n, void* Element, int szElement) //create subdirectory
{
	CHECK(t==0,"STREE_InsertRight",return 0);
	CHECK(n==0,"STREE_InsertRight",return 0);
	SPG_TNODE* w = (SPG_TNODE*)LST_Store(&(t->S),0,sizeof(SPG_TNODE)+szElement);
	w->Parent=n;
	w->Right=n->Right;
	if(n->Right) n->Right->Parent=w;
	w->Left=0;
	w->szElement=szElement;
	n->Parent=w;
	STREE_SetData(t,w,Element,szElement);
	return w;
}

//##################          TREE UTILITY FUNCTIONS         ##############

*/