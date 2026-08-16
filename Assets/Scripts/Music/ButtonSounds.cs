using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/*
Pblm : comment on sait que la souris est en train de survoler ce bouton précis à cet instant précis ??
private void Update()
{
    // Convertir la position de la souris en coordonnées écran
    // Lancer un raycast pour savoir si ce raycast touche CE bouton précis
    // Comparer avec l'état de la frame précédente : est-ce qu'on VIENT d'entrer dans la zone (et pas juste "on est dedans depuis 10 frames") ?
    // Si oui : jouer le son
}
FASTIDIEUX! => systeme de détection tout fait par Unity à la place
Comment en etre sur ? un contrat!!

IPointerEnterHandler permet de nous brancher sur ce système de détection qui existe deja sans en connaitre les rouages
puis avec OnPointerEnter(PointerEventData eventData) on dit à unity
 : quand TON systeme interne détecte que la souris vient d'entrer sur mon gameobject, appelle cette méthode chez moi.

INTERFACE
= contrat, pas de code juste une promesse que toute classe qui implémente cette interface
doit fournir une méthode avec cette signature précise

EXPRESSION
button.onClick.AddListener(() =>
    SFXManager.Instance.PlaySFX(SFXManager.Instance.clickClip));

Evenements Unity UI : ONCLICK & ADDLISTENER
button.onClick = evenement, liste d'actions à executer quand le bouton est cliqué
AddListener = ajoute une action à cette liste sans écraser les actions qui existent deja 
=> pas comme les actions qu'on rajoute dans l'inspecteur, ici on les ajoute par le code au moment de Awake()

() => ... FONCTION ANONYME
ptt méthode qu'on def rapido sans nom directement là ou elle est utilisé
une fonction qui ne prend aucun paramètre () et qui quand on l'appele exécute (=>) cette instruction

donc AddListener attend en parametre une méthode à execute plus tard quand le click arrivera 
équivalent à écrire : 

private void PlayClickSound()
{
    SFXManager.Instance.PlaySFX(SFXManager.Instance.clickClip);
}
// puis
button.onClick.AddListener(PlayClickSound);

*/


// attribut unity spéciale : dit a unity si qql ajoute ce script sur un gameobject qui n'a pas deja un composant Button
// alors ajoute directement un composant button a celui ci 
[RequireComponent(typeof(Button))]

// herite de 2 choses : MOnobehavior = classe de base normale, IPointerHandler = interface
public class ButtonSFX : MonoBehaviour, IPointerEnterHandler
{
    private Button button;

    // initialisation (1 fois puis Update) : va chercher les composants dont j'ai besoin, prépare mes variables, branches mes abonnements
    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() =>
            SFXManager.Instance.PlaySFX(SFXManager.Instance.clickClip)); // !
    }

    // exige cette méthode = Unity appele automatiquement OnPointerEnter() dès que la souris entre dans la zone du bouton
    public void OnPointerEnter(PointerEventData eventData) // eventData = info sur l'event
    {
        if (!button.interactable) return; // si le puzzle est verouillé on ne fait pas hover
        SFXManager.Instance.PlaySFX(SFXManager.Instance.hoverClip);
    }
}