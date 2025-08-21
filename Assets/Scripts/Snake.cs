using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Snake : MonoBehaviour
{
    [Header("Configurações do Jogo")]
    [SerializeField] List<Questao> questoes;
    [SerializeField] int questaoAtual = 0;
    [SerializeField] Text UIenunciado;
    [SerializeField] Text UIpontos;
    [SerializeField] GameObject LOGO;
    [SerializeField] GameObject UIdicas;
    [SerializeField] GameObject gameOver;
    [SerializeField] Text UIgameover;

    [Header("Configurações de Som")]
    [SerializeField] AudioSource trilhaSonora;
    [SerializeField] AudioSource coletar;

    [Header("Configurações da Cobra")]
    [SerializeField] int pontos = 0;
    [SerializeField] Vector2 direction;
    [SerializeField] List<Transform> corpo;
    [SerializeField] Sprite cabeca;
    [SerializeField] Sprite rabo;
    [SerializeField] List<Sprite> peles;
    [SerializeField] Sprite corpo_curva;
    [SerializeField] GameObject corpo_modelo;

    void Start()
    {
        Time.timeScale = 1; // Garantir que o jogo comece em velocidade normal
        trilhaSonora.Play();
        LOGO.SetActive(true);
        UIdicas.SetActive(false);
        UIenunciado.text = "";
        UIpontos.text = "0 Pontos";
        pontos = 0;
        gameOver.SetActive(false);
        corpo = new List<Transform>();
        corpo.Add(transform);
        questaoAtual = UnityEngine.Random.Range(0, questoes.Count);
        UIenunciado.text = questoes[questaoAtual].pergunta;
    }

    void Update()
    {
        if (LOGO.activeSelf)
        {
            if (Input.anyKeyDown)
            {
                LOGO.SetActive(false);
            }
            return;
        }
        else
        {
            float axisX = Input.GetAxis("Horizontal");
            float axisY = Input.GetAxis("Vertical");

            int posX = Mathf.RoundToInt(axisX);
            int posY = Mathf.RoundToInt(axisY);

            if (posX != 0) direction = Vector2.right * posX;
            if (posY != 0) direction = Vector2.up * posY;

            UIenunciado.text = questoes[questaoAtual].pergunta;
            UIpontos.text = pontos + " Pontos";
        }
    }

    void FixedUpdate()
    {
        for (int i = corpo.Count - 1; i > 0; i--)
        {
            corpo[i].position = corpo[i - 1].position;
            corpo[i].eulerAngles = corpo[i - 1].eulerAngles;
            //Verificar diagonal
            if (corpo[i].position.x == corpo[i - 1].position.x && corpo[i].position.y == corpo[i - 1].position.y)
            {
                corpo[i].GetComponent<SpriteRenderer>().sprite = peles[UnityEngine.Random.Range(0, peles.Count)];
            }
            else
            {
                corpo[i].GetComponent<SpriteRenderer>().sprite = corpo_curva;
            }
        }
        corpo[0].GetComponent<SpriteRenderer>().sprite = cabeca;
        if (corpo.Count > 1) corpo[corpo.Count - 1].GetComponent<SpriteRenderer>().sprite = rabo;
        Movimento();
    }

    void Movimento()
    {
        int posX = Mathf.RoundToInt(transform.position.x);
        int posY = Mathf.RoundToInt(transform.position.y);

        if (direction.y > 0) transform.eulerAngles = new Vector3(0, 0, 90);
        else if (direction.y < 0) transform.eulerAngles = new Vector3(0, 0, -90);
        else if (direction.x > 0) transform.eulerAngles = new Vector3(0, 0, 0);
        else if (direction.x < 0) transform.eulerAngles = new Vector3(0, 0, 180);

        transform.position = new Vector2(posX + direction.x, posY + direction.y);
            // Verifica colisão com o próprio corpo
            for (int i = 1; i < corpo.Count; i++)
            {
                if (transform.position == corpo[i].position)
                {
                    direction = Vector2.zero; // Parar a cobra
                    UIgameover.text = "Você fez " + pontos + " pontos";
                    gameOver.SetActive(true);
                    Time.timeScale = 0; // Pausar o jogo
                    break;
                }
            }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Comida"))
        {
            GameObject objeto = collision.gameObject;
            string resposta = questoes[questaoAtual].resposta;
            if (objeto.name == "Fruta" + resposta)
            {
                pontos++;
                questaoAtual = UnityEngine.Random.Range(0, questoes.Count);
                Groing();
                coletar.Play();
            }
            else
            {
                direction = Vector2.zero; // Parar a cobra
                UIgameover.text = "Você fez " + pontos + " pontos";
                gameOver.SetActive(true);
                Time.timeScale = 0; // Pausar o jogo
            }

        }

        if(collision.CompareTag("Parede"))
        {
            direction = Vector2.zero; // Parar a cobra
            UIgameover.text = "Você fez " + pontos + " pontos";
            gameOver.SetActive(true);
            Time.timeScale = 0; // Pausar o jogo
        }
    }

    public void Pausar()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            trilhaSonora.Pause();
        }
        else
        {
            Time.timeScale = 1;
            trilhaSonora.Play();
        }
    }

    public void DicasControles()
    {
        UIdicas.SetActive(!UIdicas.activeSelf);
    }

    public void Restart()
    {
        SceneManager.LoadScene("MAIN");
    }
    
    void Groing()
    {
        GameObject novoCorpo = Instantiate(corpo_modelo, corpo[corpo.Count - 1].position, Quaternion.identity);
        corpo.Add(novoCorpo.transform);
    }
}
