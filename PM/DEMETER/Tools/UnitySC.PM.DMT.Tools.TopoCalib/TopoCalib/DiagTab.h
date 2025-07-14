#ifndef _DIAGONGLET_H_
#define _DIAGONGLET_H_

/* ****************** INCLUDES ******************* */
//#include "afxcmn.h"

/** Utilisation
    Insérer la classe CDiagTab dans le projet
    Dans la boite de dialogue hôte, déclarer :
    un contrôle CTabCtrl et lui associer une variable contrôle 'm_Onglet'
    modifier manuellement le type de la variable associée en 'CDiagTab'
    Construire les ressources de toutes le boites de dialogue des pages et les classes associées
    Dans la fonction InitDialogue de la boite hôte :
    	- Créer les boites de dialogues : Dlg.Create(IDD_DIALOG, &m_Onglet);
    	_ les associer aux pages : m_Onglet.InsertItem(indice, (CDialog *) Dlg, "Titre onglet");
*/
class CDiagTab : public CTabCtrl
{
// ----------- TYPES -----------
private:
	enum {ONG_NBMAX = 16,};

// ----------- ATTRIBUTS -----------
private:
	int		m_nNbItem;
	int		m_CurDlg;
	CDialog *TabDlg[ONG_NBMAX];

// ----------- METHODES -----------
public:
    /** Constructeur.   
	 */
			CDiagTab();

    /** Destructeur.   
	 */
	virtual ~CDiagTab();

    /** Insertion d'un nouvel onglet au numéro nItem.
		@return -1 si erreur.
	 */
	int		InsertItem(int nItem, CDialog *pDlg, CString sCaption);

    /** supression de l'onglet numéro nItem (décalage des suivants).
	 */
	BOOL	DeleteItem(int nItem);

    /** renomme l'onglet d'indice nItem.
	 */
	void	Rename(int nItem, CString sCaption);

    /** Active et affiche la boite de dialogue associée à l'onglet d'indice nItem.
		@return indice de l'onglet précédemment sélecté.
	 */
	int		SetCurSel(int nItem = -1);

    /** Retourne le pointeur vers la CDialog d'indice spécifié.
	@param nItem Indice de l'onglet.
		@return indice de l'onglet précédemment sélecté.
	 */
inline	CDialog *		GetDialog(int nItem) 
			{ return( ((nItem >=0) && (nItem < m_nNbItem)) ? TabDlg[nItem] : NULL); };

private:
	void	Hide();

protected:
	// ClassWizard generated virtual function overrides
	//{{AFX_VIRTUAL(COnglet)
	//}}AFX_VIRTUAL

	//{{AFX_MSG(COnglet)
	afx_msg void OnClick(NMHDR* pNMHDR, LRESULT* pResult);
	//}}AFX_MSG

	DECLARE_MESSAGE_MAP()
};

#endif //#ifndef _DIAGONGLET_H_
