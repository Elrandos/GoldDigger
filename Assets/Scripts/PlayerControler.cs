using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class PlayerControler : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool isMoving;
    public int tilesDug = 0;
    public TMP_Text tilesDugText;
    public Vector2 input;

    private Coroutine moveCoroutine;
    private Rigidbody2D rb;

    private Animator animator;

    public LayerMask solidObjectsLayer;

    public Tilemap solidTilemap;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            // Input 10x mniejszy dla precyzyjniejszego ruchu
            input.x = Input.GetAxisRaw("Horizontal") / 10;
            input.y = Input.GetAxisRaw("Vertical") / 10;

            if (input.x != 0 && input.y != 0)
            {
                //Więcej obliczeń, procesor to wytrzyma!
                input.x = input.x / 1414 * 1000;
                input.y = input.y / 1414 * 1000;
            }

            if (input != Vector2.zero)
            {
                Vector2 targetPosition = rb.position + input;

                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                }

                if (IsWalkable(targetPosition))
                {
                    moveCoroutine = StartCoroutine(Move(targetPosition));
                }
            }
        }

        // Ustawianie kierunku patrzenia na podstawie kursora
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 lookDir = mouseWorldPos - transform.position;
        lookDir.Normalize();

        // Wcześniej dzieliłem przez 10 dla płynniejszego ruchu, także teraz *10, żeby animator miał właściwą wartość (1)
        animator.SetFloat("moveX", Mathf.Round(lookDir.x * 10));
        animator.SetFloat("moveY", Mathf.Round(lookDir.y * 10));

        animator.SetBool("isMoving", isMoving);

        if (Input.GetMouseButtonDown(0))
        {
            Dig();
        }
    }


    void Dig()
    {
        // Sprawdzamy, czy liczba wykopanych kafelków wynosi 6
        if (tilesDug >= 6)
        {
            EndGame();  // Zakończenie gry
            return;  // Zatrzymujemy dalsze wykopywanie
        }

        var facingDirection = new Vector3(animator.GetFloat("moveX"), animator.GetFloat("moveY"));
        var interactionPosition = transform.position + facingDirection / 10;

        Debug.DrawLine(transform.position, interactionPosition, Color.red, 1f);

        var collider = Physics2D.OverlapCircle(interactionPosition, 0.1f, solidObjectsLayer);

        if (collider != null)
        {
            Vector3Int cellPos = solidTilemap.WorldToCell(interactionPosition);
            if (solidTilemap.HasTile(cellPos))
            {
                solidTilemap.SetTile(cellPos, null);
                tilesDug++;
                UpdateUI();
            }
        }
    }

    void UpdateUI()
    {
        if (tilesDugText != null)
        {
            tilesDugText.text = "Wykopane: " + tilesDug.ToString();
        }

        // Dodajemy sprawdzenie w UI, czy osiągnięto 6 wykopanych kafelków
        if (tilesDug >= 6)
        {
            // Możemy wyświetlić napis "KONIEC GRY" na UI
            tilesDugText.text = "KONIEC GRY! Wykopałeś 6 kafelków.";
        }
    }

    void EndGame()
    {
        Debug.Log("KONIEC GRY! Wykopałeś 6 kafelków.");

        isMoving = false;
        Time.timeScale = 0;
    }


    IEnumerator Move(Vector2 targetPosition)
    {
        isMoving = true;

        while (Vector2.Distance(rb.position, targetPosition) > 0.01f)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPos);
            yield return null;
        }

        rb.MovePosition(targetPosition);
        isMoving = false;
    }

    private bool IsWalkable(Vector3 targetPosition)
    {
        if (Physics2D.OverlapCircle(targetPosition, 0.2f, solidObjectsLayer) != null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
