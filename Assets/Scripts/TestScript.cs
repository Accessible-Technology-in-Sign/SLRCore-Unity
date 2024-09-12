using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    private SLRTfLiteModel<string> recogniser;

    [SerializeField] private TextAsset _modelFile;
    [SerializeField] private TextAsset _mappingFile;
    

    // Start is called before the first frame update
    void Awake()
    {
        string[] mapping = _mappingFile.text.Split("\n");
        for (int i = 0; i < mapping.Length; i++) {
            mapping[i] = mapping[i].Trim().ToLower();
        } 
        recogniser = new SLRTfLiteModel<string>(_modelFile, mapping);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
