using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class EMGfromArduino : MonoBehaviour
{
    /// <summary>
    ///  This script changes between grasp/pinch type, manages the animation of fingers and EMG CALIBRATION.
    /// </summary>


    private TextMeshProUGUI lblGuidelines;

    [SerializeField]
    /// <summary>
    /// 0 - Vive Controller trigger
    /// 1 - Keyboard (P and O)
    /// 2 - EMG
    /// </summary>
    private int m_InputType = 0;
    private int m_GraspType = 1;
    [SerializeField]
    private string m_PortName;
    public float m_FrozenValue = 101, m_LastProsthesisValue = 0;
    public string m_HandStatus = "Opened";
    private Coroutine handMovementCoroutine;
    [SerializeField]
    private bool isOpenHandCoroutineRunning = false, isCloseHandCoroutineRunning = false;

    #region Variables to control EMG
    private ArduinoManager _arduino;

    [SerializeField]
    float minValueToClose = 0;

    [SerializeField]
    float[] emgRange;

    private float maxAperture = 0, minAperture = 0;
    [SerializeField]
    private bool isFrozen = false;
    private bool maxConfigured = false, minConfigured = false;
    public bool _calibrating = false;

    //[SerializeField]
    //private string portName = "";

    #endregion

    //Debug variables - Keyboard
    public float rotationDebbugingValue = 0;

    void Start()
    {

        #region Start Arduino connection
        _arduino = new ArduinoManager(m_PortName, 115200);
        //Debug.Log("Porta arduino selecionada::" + m_PortName);
        try
        {
            _arduino.Start();
        }
        catch (Exception e)
        {
            Debug.Log("Erro ao iniciar EMG: " + e);
            throw;
        }
        #endregion



        //minValueToClose = emgRange[0];
        //minValueToClose = txt.GetEMGLimiar();
        //Debug.Log("Limiar EMG: " + minValueToClose);

        //minValueToClose = minValueToClose + (minValueToClose * 0.1f);



    }

    private void FixedUpdate()
    {

        // Get data from EMG 
        #region InputType  =  EMG
        
        
            //float value = _arduino.GetData();
            float value = _arduino.GetData();
            Debug.Log("EMG:\t" + value);

            if (value > minValueToClose)
            {
                //ExecuteHandMovement();
                //ExecuteHandMovement("Close");
            }
            if (value <= minValueToClose)
            {
                //ExecuteHandMovement();
                //ExecuteHandMovement("Open");
            }

            //if (Input.GetKeyDown(KeyCode.Y)) {
            //    ExecuteHandMovement("Open");
            //}
        
        #endregion
    }


    void Update()
    {
        // if it is using EMG, then must calibrate

/*        if (m_InputType == 2)
        {
            if (_calibrating)
                return;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("KeyDown: V - Vibrar");
            _arduino.SendData("22");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("KeyDown: S - Parar de Vibrar");
            _arduino.SendData("33");
        }
*/
    }



    #region EMG DATA Calibration / Send DATA

   /* IEnumerator CalibrateClosed()
    {
        Debug.Log("-------------->Calibrando pose prótese fechada...");
        
        float value = 0;
        int samples = 500;
        int i = 0;
        while (i < samples)
        {
            float v = _arduino.GetData();
            if (!float.IsNaN(v))
            {
                value += v;
                i++;
                //Debug.Log("CalibrateClosed: " + value);
            }
            yield return null;
        }

        // Pega o valor mediano
        value = value / samples;
        // Reduz 10% do valor mediano, para garantir mais flexibilidade para o usuário
        value = value - (value * 0.1f);

        SetMaxAperture(value);
        _calibrating = false;

        
        StopCoroutine("CalibrateClosed");
        
        Debug.Log("--------------->Calibrado pose prótese fechada: " + value);
    }

    IEnumerator CalibrateOpened()
    {
        Debug.Log("-------------->Calibrando pose prótese aberta...");
        
        float value = 0;
        int samples = 500;
        int i = 0;
        while (i < samples)
        {
            float v = _arduino.GetData();
            if (!float.IsNaN(v))
            {
                value += v;
                i++;
                //Debug.Log("CalibrateOpened: " + value);
            }
            yield return null;
        }

        // Pega o valor mediano
        value = value / samples;
        // Aumenta 10% do valor mediano, para garantir mais flexibilidade para o usuário
        value = value + (value * 0.1f);

        SetMinAperture(value);
        _calibrating = false;

        
        StopCoroutine("CalibrateOpened");
        
        Debug.Log("--------------->Calibrado pose prótese aberta: " + value);
    }
*/
    private void OnApplicationQuit()
    {
        if (m_InputType == 2)
        {
            try
            {
                if (_arduino.IsRunning())
                {
                    _arduino.Stop();
                }
                Debug.Log(" conexão ENCERRADA - ARDUINO");
            }
            catch (Exception e)
            {
                Debug.Log("Erro ao encerrar conexão: " + e);
            }
        }
    }

    /// <summary>
	/// Calibrates the maximum data from the device to be the maximum closing
	/// </summary>
	/// <param name="max"></param>
	void SetMaxAperture(float max)
    {
        maxAperture = max;
        maxConfigured = true;
    }

    /// <summary>
    /// Calibrates the minimum data from the device to be the maximum opening
    /// </summary>
    /// <param name="min"></param>
    void SetMinAperture(float min)
    {
        minAperture = min;
        minConfigured = true;
    }

    public void SendDataToArduino(string v)
    {
        _arduino.SendData(v);
    }

    #endregion
}
