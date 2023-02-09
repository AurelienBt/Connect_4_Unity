using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Indique si l'on joue contre l'IA ou non
    public static bool playAgainstAI = true;

    bool isPlayer, hasGameFinished;

    // Jeton rouge
    [SerializeField] // permet d'acc�der aux variables dans l'inspecteur de Unity
    GameObject red;

    // Jeton jaune
    [SerializeField]
    GameObject yellow;

    // Texte indiquant qui doit jouer
    [SerializeField]
    Text TXT_Tour;

    // Couleurs de texte
    Color RED_COLOR = new Color(214, 33, 33, 255) / 255;
    Color YELLOW_COLOR = new Color(238, 228, 41, 255) / 255;

    Plateau myBoard;

    private void Awake()
    {
        isPlayer = true;
        hasGameFinished = false;
        myBoard = new Plateau();
    }

    // Charge la sc�ne de menu
    public void GameStart()
    {
        SceneManager.LoadScene("Menu");
    }

    public void JoueurVsJoueur()
    {
        playAgainstAI = false;
        SceneManager.LoadScene("Game");
    }

    public void JoueurVsIA()
    {
        playAgainstAI = true;
        SceneManager.LoadScene("Game");
    }

    public void Rejouer()
    {
        SceneManager.LoadScene("Game");
    }

    public void Quitter()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }

    // Fonction appel�e � chaque frame pour update le jeu
    private void Update()
    {   
        // Tour de l'IA
        if (playAgainstAI && !isPlayer)
        {
            if (hasGameFinished) return;

            // Recherche du meilleur mouvement
            int numCol = myBoard.GetBestMove(myBoard, 5);

            // Placement du jeton
            string nomCol = "Colonne" + numCol;
            GameObject col = GameObject.Find(nomCol); 
            Colonne c = col.GetComponent<Colonne>();
            Vector3 spawnPos = c.GetComponent<Colonne>().spawnLocation;
            Vector3 targetPos = c.GetComponent<Colonne>().targetLocation;
            GameObject circle = Instantiate(isPlayer ? red : yellow);
            circle.transform.position = spawnPos;
            circle.GetComponent<Mouvement>().targetPostion = targetPos;

            // Augmente la hauteur min de la colonne (� cause du nouveau jeton plac�)
            c.GetComponent<Colonne>().targetLocation = new Vector3(targetPos.x, targetPos.y + 54f, targetPos.z);

            // Met � jour la grille en m�moire
            myBoard.UpdateBoard(c.GetComponent<Colonne>().col - 1, isPlayer);

            // V�rifie s'il y a un gagnant
            if (myBoard.Result(isPlayer))
            {
                TXT_Tour.text = "Le " + (isPlayer ? "Rouge" : "Jaune") + " Gagne !";
                hasGameFinished = true;
                return;
            }
            // V�rifie si le plateau est rempli
            else if (myBoard.IsFull())
            {
                TXT_Tour.text = "�galit� !";
                TXT_Tour.color = Color.white;
                hasGameFinished = true;
                return;
            }

            // Message de tour
            TXT_Tour.text = !isPlayer ? "Au tour du Rouge" : "Au tour du Jaune";
            TXT_Tour.color = !isPlayer ? RED_COLOR : YELLOW_COLOR;

            // Changement de joueur
            isPlayer = !isPlayer;
        }
        // Tour du joueur humain
        else
        {
            // Clic de souris
            if (Input.GetMouseButtonDown(0))
            {
                if (hasGameFinished) return;

                // Raycast2D des coordonn�es de la souris pour voir si l'on croise une zone de collision
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 mousePos2D = new(mousePos.x, mousePos.y);
                RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
                if (!hit.collider) return;

                // V�rifie que la zone de collision touch�e soit bien une colonne
                if (hit.collider.CompareTag("appui"))
                {
                    // V�rifie que l'on ne d�passe pas de la colonne (colonne d�j� remplie)
                    if (hit.collider.gameObject.GetComponent<Colonne>().targetLocation.y > 350f) return;

                    // Placement du jeton
                    Vector3 spawnPos = hit.collider.gameObject.GetComponent<Colonne>().spawnLocation;
                    Vector3 targetPos = hit.collider.gameObject.GetComponent<Colonne>().targetLocation;
                    GameObject circle = Instantiate(isPlayer ? red : yellow);
                    circle.transform.position = spawnPos;
                    circle.GetComponent<Mouvement>().targetPostion = targetPos;

                    // Augmente la hauteur min de la colonne (� cause du nouveau jeton plac�)
                    hit.collider.gameObject.GetComponent<Colonne>().targetLocation = new Vector3(targetPos.x, targetPos.y + 54f, targetPos.z);

                    // Met � jour la grille en m�moire
                    myBoard.UpdateBoard(hit.collider.gameObject.GetComponent<Colonne>().col - 1, isPlayer);

                    // V�rifie s'il y a un gagnant
                    if (myBoard.Result(isPlayer))
                    {
                        TXT_Tour.text = "Le " + (isPlayer ? "Rouge" : "Jaune") + " Gagne !";
                        hasGameFinished = true;
                        return;
                    }
                    // V�rifie si le plateau est rempli
                    else if (myBoard.IsFull())
                    {
                        TXT_Tour.text = "�galit� !";
                        TXT_Tour.color = Color.white;
                        hasGameFinished = true;
                        return;
                    }

                    // Message de tour
                    TXT_Tour.text = !isPlayer ? "Au tour du Rouge" : "Au tour du Jaune";
                    TXT_Tour.color = !isPlayer ? RED_COLOR : YELLOW_COLOR;

                    // Changement de joueur
                    isPlayer = !isPlayer;
                }

            }
        }
    }
}