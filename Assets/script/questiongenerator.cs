using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class questiongenerator : MonoBehaviour
{
    public TMP_Text question;
    public GameObject[] answers;
    public GameObject questionObject;
    string[] format = {"plus", "minus", "x", "bagi"};
    bool opened = false;
    public void StartQuiz()
    {
        if(opened == false){
            questionObject.SetActive(true);
            int rnd = Random.Range(0, 4);
            int a = Random.Range(1, 10);
            int b = Random.Range(1, 10);
            float answer = 0;
            switch(rnd){
                case 0:
                    answer = a + b;
                    question.text = a.ToString() + " + " + b.ToString();
                    break;
                case 1:
                    answer = a - b;
                    question.text = a.ToString() + " - " + b.ToString();
                    break;
                case 2:
                    answer = a * b;
                    question.text = a.ToString() + " x " + b.ToString();
                    break;
                case 3:
                    answer = a / b;
                    question.text = a.ToString() + " / " + b.ToString();
                    break;
            }
            List<float> wronganswerlist = new List<float>();
            foreach(GameObject go in answers){
                float wronganswer = 0;
                do{
                    wronganswer = answer + Random.Range(-3, 3);
                }while(wronganswerlist.Contains(wronganswer));
                wronganswerlist.Add(wronganswer);
                go.GetComponent<answerlogic>().init(this);
                go.transform.GetChild(0).GetComponent<TMP_Text>().text = wronganswer.ToString();
            }

            GameObject go1 = answers[Random.Range(0, answers.Length)];
            go1.GetComponent<answerlogic>().correctanswer = true;
            go1.transform.GetChild(0).GetComponent<TMP_Text>().text = answer.ToString();
            opened = true;
        }
    }
}
