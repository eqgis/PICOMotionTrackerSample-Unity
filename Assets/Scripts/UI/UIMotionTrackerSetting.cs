/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2024 PICO Developer
// SPDX-License-Identifier: MIT
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using UnityEngine.UI;
using TMPro;
using MotionTracking;
using UnityEngine.Events;
using System;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;

public class UIMotionTrackerSetting : MonoBehaviour
{
    enum SelectModelName
    {
        Default=0,
        TennisRacket=1,
        TableTennisBats=2,
        BaseballBat=3,
        ToyGun=4,
        ToyCar=5,
        Vase=6,
        CokeCan=7
    }
    enum SelectmotionTrackerAssisted
    {
        None,
        Assisted1,
        Assisted2
    }
    // Start is called before the first frame update

    public TMP_Text trackerName;
    public TMP_Dropdown dropdownModel;
    public TMP_Dropdown dropdownModelAssisted;
    public UIMotionTrackerSetting[] motionTrackerSetting;
    [HideInInspector]
    public MotionTrackerSampler motionTrackerSampler;
    public GameObject gameObjectModelSetting;
    [HideInInspector]
    public GameObject gameObjectMotionTrackerAssisted;
    [HideInInspector]
    public MotionTrackerSampler motionTrackerSamplerAssisted1;
    [HideInInspector]
    public MotionTrackerSampler motionTrackerSamplerAssisted2;
    public GameObject template;
    public SelectGameObjectModel selectGameObjectModel;
    [SerializeField]
    private TMP_Text[] offestInformation;
    [SerializeField]
    private Button btnBindingModel;
    [SerializeField] 
    private Button btnRestModel;
    [SerializeField]
    private Button btnBindingAssisted;
    [SerializeField]
    private UIWriteConfig uIWriteConfig;
    [SerializeField]
    private Button btnPosXPlus;
    [SerializeField]
    private Button btnPosXMinus;
    [SerializeField]
    private Button btnPosYPlus;
    [SerializeField]
    private Button btnPosYMinus;
    [SerializeField]
    private Button btnPosZPlus;
    [SerializeField]
    private Button btnPosZMinus;
    [SerializeField]
    private Button btnRowXPlus;
    [SerializeField]
    private Button btnRowXMinus;
    [SerializeField]
    private Button btnRowYPlus;
    [SerializeField]
    private Button btnRowYMinus;
    [SerializeField]
    private Button btnRowZPlus;
    [SerializeField]
    private Button btnRowZMinus;
    private SelectModelName modelName;
    private SelectmotionTrackerAssisted motionTrackerAssisted;
    private int selectModelIndex;
    [SerializeField]
    private UIReadJson uIReadJson;
    private bool motiontrackerIsBind=true;
    private bool motiontrackerAssistedIsBind1;
    private bool motiontrackerAssistedIsBind2;
    private bool isReadJson;
    private bool isReadJsonIndex;
    private int jsonModelIndex;
    [SerializeField]
    private GameObject offestBtn;
    [SerializeField]
    private TMP_Text textStatus;
    [SerializeField]
    private TMP_Text textStatusAssist;
    private Vector3 offestTrackerRotation;
    private Vector3 offestTrackerPostion;
    private Vector3 startTrackerRotation;
    private Vector3 startTrackerPostion;
    private Vector3 offestPos;
    private Vector3 offestRow;
    private Vector3 offestPosSum;
    private Vector3 offestRowSum;
    private Vector3 offestPosAss;
    private Vector3 offestRowAss;
    private Vector3 offestPosAssModel;
    private Vector3 offestRowAssModel;
    private bool bindPartent=true;
    private bool isSelectPen;
    private void Awake()
    {
        dropdownModel.onValueChanged.AddListener(SelectModel);
        dropdownModelAssisted.onValueChanged.AddListener(SelectAssistedTracker);
        btnBindingModel.onClick.AddListener(BtnManualCalibration);
        btnRestModel.onClick.AddListener(BtnRestOnClick);
        btnBindingAssisted.onClick.AddListener(SetMotionTrackerAssistedPartent);
        btnPosXPlus.onClick.AddListener(OnPosxPlus);
        btnPosXMinus.onClick.AddListener(OnPosxMinus);
        btnPosYPlus.onClick.AddListener(OnPosYPlus);
        btnPosYMinus.onClick.AddListener(OnPosYMinus);
        btnPosZPlus.onClick.AddListener(OnPosZPlus);
        btnPosZMinus.onClick.AddListener(OnPosZMinus);
        btnRowXPlus.onClick.AddListener(OnRowxPlus);
        btnRowXMinus.onClick.AddListener(OnRowxMinus);
        btnRowYPlus.onClick.AddListener(OnRowYPlus);
        btnRowYMinus.onClick.AddListener(OnRowYMinus);
        btnRowZPlus.onClick.AddListener(OnRowZPlus);
        btnRowZMinus.onClick.AddListener(OnRowZMinus);
    }
    void Start()
    {
        uIReadJson.readJsonTrackerAction += ReadJsonData;
        uIReadJson.readJsonTrackerAssistAction += ReadJsonDataAssist;
        motionTrackerSampler.trackerConfidenceAction += OnConfidence;
        motionTrackerSampler.trackerConfidenceActionPen += OnTrackerCallBack;
    }

    private void OnDestroy()
    {
        uIReadJson.readJsonTrackerAction -= ReadJsonData;
        uIReadJson.readJsonTrackerAssistAction -= ReadJsonDataAssist;
        motionTrackerSampler.trackerConfidenceAction -= OnConfidence;
    }
    // Update is called once per frame
    void Update()
    {
        if (motionTrackerSampler==null)
        {
            return;
        }
        if (!isReadJson)
        {
            if (!motiontrackerIsBind)
            {
                offestTrackerPostion = motionTrackerSampler.transform.position - startTrackerPostion;
                offestTrackerRotation = motionTrackerSampler.transform.rotation.eulerAngles;
                offestInformation[0].text = ((int)((offestTrackerPostion.x + offestPos.x) * 100)).ToString();
                offestInformation[1].text = ((int)((offestTrackerPostion.y + offestPos.y) * 100)).ToString();
                offestInformation[2].text = ((int)((offestTrackerPostion.z + offestPos.z) * 100)).ToString();
                offestInformation[3].text = ((int)(offestTrackerRotation.x + offestRow.x)).ToString();
                offestInformation[4].text = ((int)(offestTrackerRotation.y + offestRow.y)).ToString();
                offestInformation[5].text = ((int)(offestTrackerRotation.z + offestRow.z)).ToString();
            }
            if (motionTrackerAssisted == SelectmotionTrackerAssisted.Assisted1 && !motiontrackerAssistedIsBind1)
            {
                offestInformation[6].text = ((int)((motionTrackerSampler.transform.position.x * 100)- motionTrackerSamplerAssisted1.transform.position.x * 100)).ToString();
                offestInformation[7].text = ((int)((motionTrackerSampler.transform.position.y * 100)- motionTrackerSamplerAssisted1.transform.position.y * 100)).ToString();
                offestInformation[8].text = ((int)((motionTrackerSampler.transform.position.z * 100)- motionTrackerSamplerAssisted1.transform.position.z * 100)).ToString();
                offestInformation[9].text = ((int)(motionTrackerSampler.transform.rotation.eulerAngles.x - motionTrackerSamplerAssisted1.transform.rotation.eulerAngles.x)).ToString();
                offestInformation[10].text = ((int)(motionTrackerSampler.transform.rotation.eulerAngles.y-motionTrackerSamplerAssisted1.transform.rotation.eulerAngles.y)).ToString();
                offestInformation[11].text = ((int)(motionTrackerSampler.transform.rotation.eulerAngles.z - motionTrackerSamplerAssisted1.transform.rotation.eulerAngles.z)).ToString();
                offestPosAss = motionTrackerSampler.transform.position - motionTrackerSamplerAssisted1.transform.position;
                offestRowAss = motionTrackerSampler.transform.rotation.eulerAngles - motionTrackerSamplerAssisted1.transform.rotation.eulerAngles;
            }
            else if (motionTrackerAssisted == SelectmotionTrackerAssisted.Assisted2 && !motiontrackerAssistedIsBind2)
            {
                offestInformation[6].text = ((int)((motionTrackerSampler.transform.position.x * 100) - motionTrackerSamplerAssisted2.transform.position.x * 100)).ToString();
                offestInformation[7].text = ((int)((motionTrackerSampler.transform.position.y * 100) - motionTrackerSamplerAssisted2.transform.position.y * 100)).ToString();
                offestInformation[8].text = ((int)((motionTrackerSampler.transform.position.z * 100) - motionTrackerSamplerAssisted2.transform.position.z * 100)).ToString();
                offestInformation[9].text = ((int)(motionTrackerSampler.transform.rotation.eulerAngles.x - motionTrackerSamplerAssisted2.transform.rotation.eulerAngles.x)).ToString();
                offestInformation[10].text = ((int)(motionTrackerSampler.transform.rotation.eulerAngles.y - motionTrackerSamplerAssisted2.transform.rotation.eulerAngles.y)).ToString();
                offestInformation[11].text = ((int)(motionTrackerSampler.transform.rotation.eulerAngles.z - motionTrackerSamplerAssisted2.transform.rotation.eulerAngles.z)).ToString();
                offestPosAss = motionTrackerSampler.transform.position - motionTrackerSamplerAssisted2.transform.position;
                //offestRowAss = motionTrackerSampler.transform.rotation.eulerAngles - motionTrackerSamplerAssisted2.transform.rotation.eulerAngles;
                offestRowAss = motionTrackerSampler.transform.rotation.eulerAngles;
            }
        }
        offestPosSum = offestTrackerPostion + offestPos;
        offestRowSum = offestTrackerRotation + offestRow;
    }
    /// <summary>
    /// 选择模型，每次选择时先解绑解绑父子关系，选择模型后将tracker的坐标赋值到模型上
    /// 并把tracker的坐标进行记录。
    /// </summary>
    /// <param name="index"></param>
    public void SelectModel(int index)
    {
        if(motionTrackerSampler==null)
        {
            return;
        }
        if (index == 0)
        {
            gameObjectModelSetting.SetActive(false);
            gameObjectMotionTrackerAssisted.SetActive(false);
            dropdownModel.value = 0;
            dropdownModelAssisted.value = 0;
            selectModelIndex = index;
            template.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1);
            modelName = 0;
            selectGameObjectModel.RemoveBinding();
            selectGameObjectModel.SelectModel(index, motionTrackerSampler._transform);
            if (motiontrackerIsBind && !isReadJson)
            {
                selectGameObjectModel.RestModelTransform();
                motiontrackerIsBind = false;
            }
            if (motiontrackerAssistedIsBind1)
            {
                //motionTrackerSamplerAssisted1.gameObject.transform.SetParent(null);
                motiontrackerAssistedIsBind1 = false;
                motionTrackerSamplerAssisted1.isBindAss = false;
            }
            else if (motiontrackerAssistedIsBind2)
            {
                //motionTrackerSamplerAssisted2.gameObject.transform.SetParent(null);

                motiontrackerAssistedIsBind2 = false;
                motionTrackerSamplerAssisted2.isBindAss = true;
            }
            motionTrackerAssisted = SelectmotionTrackerAssisted.None;
            isReadJson = false;
            motionTrackerSampler.isReadJson = false;
            isSelectPen = false;
        }
        else 
        {
            if (isReadJson)
            {
                motiontrackerIsBind = false;
            }
            dropdownModel.value = index;
            selectModelIndex = index;
            gameObjectModelSetting.SetActive(true);
            template.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            selectGameObjectModel.RemoveBinding();
            selectGameObjectModel.SelectModel(index, motionTrackerSampler._transform);
            motiontrackerIsBind = true;
            startTrackerPostion = motionTrackerSampler.transform.position;
            startTrackerRotation = motionTrackerSampler.transform.rotation.eulerAngles;
            uIWriteConfig.selectModelName = modelName.ToString();
            uIWriteConfig.SaveMotionTrackerTransform(startTrackerPostion, startTrackerRotation, motionTrackerSampler.id.value, selectModelIndex);
            UpdateStatusText();
            offestBtn.SetActive(false);
            if (dropdownModel.options[index].text == "Pen")
            {
                isSelectPen = true;
                Debug.Log("LakerPen");
            }
            else
            {
                isSelectPen = false;
            }
        }
        for (int i = 0; i < offestInformation.Length; i++)
        {
            offestInformation[i].text= 0.ToString();
        }
        modelName = (SelectModelName)index;
    }
    /// <summary>
    /// 选择辅助tracker
    /// </summary>
    /// <param name="index"></param>
    public void SelectAssistedTracker(int index)
    {
        if (index == 0)
        {
            RemoveMotionTrackerAssist();
        }
        else
        {

            gameObjectMotionTrackerAssisted.SetActive(true);
            if (motionTrackerSamplerAssisted1 !=null&& (dropdownModelAssisted.options[index].text.Equals(motionTrackerSamplerAssisted1.textSN.GetComponent<TMP_Text>().text)))
            {
                dropdownModelAssisted.value = 1;
                motionTrackerAssisted = SelectmotionTrackerAssisted.Assisted1;
                motiontrackerAssistedIsBind1 = false;
                UpdateStatusTextAssist(motionTrackerSamplerAssisted1);
                motionTrackerSamplerAssisted1.gameObject.SetActive(true);
                motionTrackerSamplerAssisted1.isBindAss = true;

            }
            else if (motionTrackerSamplerAssisted2 != null && dropdownModelAssisted.options[index].text.Equals(motionTrackerSamplerAssisted2.textSN.GetComponent<TMP_Text>().text))
            {
                dropdownModelAssisted.value = 2;
                motionTrackerAssisted = SelectmotionTrackerAssisted.Assisted2;
                motiontrackerAssistedIsBind2 = false;
                UpdateStatusTextAssist(motionTrackerSamplerAssisted2);
                motionTrackerSamplerAssisted2.gameObject.SetActive(true);
                motionTrackerSamplerAssisted2.isBindAss = true;
            }
            StartCoroutine(BindingAssisted());
        }
    }
    /// <summary>
    /// 当模型绑定后，进行细微调整时使用先解绑，调整好后再次绑定
    /// </summary>
    private void BtnManualCalibration()
    {

        if (motiontrackerIsBind)
        {
            selectGameObjectModel.RemoveBinding();
            startTrackerPostion = motionTrackerSampler.transform.position;
            startTrackerRotation = motionTrackerSampler.transform.rotation.eulerAngles;
            motiontrackerIsBind = false;
            //motionTrackerSetting[1].gameObject.SetActive(true);
        }
        else
        {
            selectGameObjectModel.BindModelParent(motionTrackerSampler.transform);
            uIWriteConfig.selectModelName = modelName.ToString();
            offestTrackerPostion = motionTrackerSampler.transform.position-startTrackerPostion+offestPos;
            offestTrackerRotation = motionTrackerSampler.transform.rotation.eulerAngles+offestRow;
            uIWriteConfig.SaveMotionTrackerTransform(offestTrackerPostion,offestTrackerRotation, motionTrackerSampler.id.value, selectModelIndex);
            motiontrackerIsBind = true;
            //motionTrackerSetting[1].gameObject.SetActive(true);
        }
        UpdateStatusText();
    }
    /// <summary>
    /// 
    /// </summary>
    private void BtnRestOnClick()
    {
        for (int i = 0; i < offestInformation.Length; i++)
        {
            offestInformation[i].text = 0.ToString();
        }
        selectGameObjectModel.ResetModel(motionTrackerSampler.startPostion, motionTrackerSampler.startRotation);

    }
    private void RemoveMotionTrackerAssist()
    {
        gameObjectMotionTrackerAssisted.SetActive(false);
        if (motionTrackerAssisted == SelectmotionTrackerAssisted.Assisted1)
        {
            //motionTrackerSamplerAssisted1.gameObject.transform.SetParent(null);
            motiontrackerAssistedIsBind1 = false;
            motionTrackerSamplerAssisted1.isReadJson = false;
        }
        else if (motionTrackerAssisted == SelectmotionTrackerAssisted.Assisted2)
        {
            //motionTrackerSamplerAssisted2.gameObject.transform.SetParent(null);
            motiontrackerAssistedIsBind2 = false;
            motionTrackerSamplerAssisted2.isReadJson = false;
        }
        if (motionTrackerSamplerAssisted1 != null)
        {
            motionTrackerSetting[1].gameObject.SetActive(true);
            motionTrackerSamplerAssisted1.gameObject.SetActive(true);
        }
        if (motionTrackerSamplerAssisted2 != null)
        {
            motionTrackerSetting[2].gameObject.SetActive(true);
            motionTrackerSamplerAssisted2.gameObject.SetActive(true);
        }
        motionTrackerAssisted = SelectmotionTrackerAssisted.None;
    }
    IEnumerator BindingAssisted()
    {
        yield return null;
        if (motionTrackerAssisted == SelectmotionTrackerAssisted.Assisted1)
        {
            //motionTrackerSamplerAssisted1.gameObject.transform.SetParent(motionTrackerSampler.gameObject.transform);
            motionTrackerSetting[1].SelectModel(0);
            motionTrackerSetting[1].SelectAssistedTracker(0);
            uIWriteConfig.SaveMotionTrackerTransformAssisted(motionTrackerSampler,motionTrackerSamplerAssisted1, motionTrackerSamplerAssisted1.id.value);
            motiontrackerAssistedIsBind1 = true;
            motionTrackerSetting[1].gameObject.SetActive(false);
            //motionTrackerSamplerAssisted2.gameObject.SetActive(false);
            motionTrackerSamplerAssisted1.isBindAss = true;
            if (motionTrackerSamplerAssisted2 != null)
            {
                motionTrackerSetting[2].gameObject.SetActive(false);
                motionTrackerSamplerAssisted2.gameObject.SetActive(false);
            }
            selectGameObjectModel.BindModelParentAss(motionTrackerSamplerAssisted1.transform);
            selectGameObjectModel.BindModelParent(motionTrackerSampler.transform);
        }
        else if (motionTrackerAssisted == SelectmotionTrackerAssisted.Assisted2)
        {
            //motionTrackerSamplerAssisted2.gameObject.transform.SetParent(motionTrackerSampler.gameObject.transform);
            motionTrackerSetting[2].SelectModel(0);
            motionTrackerSetting[2].SelectAssistedTracker(0);
            motiontrackerAssistedIsBind2 = true;
            uIWriteConfig.SaveMotionTrackerTransformAssisted(motionTrackerSampler,motionTrackerSamplerAssisted2, motionTrackerSamplerAssisted2.id.value);
            motionTrackerSamplerAssisted1.gameObject.SetActive(false);
            motionTrackerSetting[1].gameObject.SetActive(false);
            motionTrackerSetting[2].gameObject.SetActive(false);
            motionTrackerSamplerAssisted1.gameObject.SetActive(false);
            selectGameObjectModel.BindModelParentAss(motionTrackerSamplerAssisted2.transform);
            selectGameObjectModel.BindModelParent(motionTrackerSampler.transform);
        }
    }
    private void SetMotionTrackerAssistedPartent()
    {
        if (motionTrackerSamplerAssisted1!=null)
        {
            if (motiontrackerAssistedIsBind1)
            {
               // motionTrackerSamplerAssisted1.gameObject.transform.SetParent(null,true);
                motiontrackerAssistedIsBind1 = false;
                //motionTrackerSamplerAssisted1.isAsMotionTrackerAssist = false;
                UpdateStatusTextAssist(motiontrackerAssistedIsBind1);
            }
            else
            {
                //motionTrackerSamplerAssisted1.gameObject.transform.SetParent(motionTrackerSampler.gameObject.transform,true);
                uIWriteConfig.SaveMotionTrackerTransformAssisted(motionTrackerSampler,motionTrackerSamplerAssisted1, motionTrackerSamplerAssisted1.id.value);
                motiontrackerAssistedIsBind1 = true;
                UpdateStatusTextAssist(motiontrackerAssistedIsBind1);
            }
        }
        if (motionTrackerSamplerAssisted2 != null)
        {
            if (motiontrackerAssistedIsBind2)
            {
                //motionTrackerSamplerAssisted2.gameObject.transform.SetParent(null,true);
                motiontrackerAssistedIsBind2 = false;
                //motionTrackerSamplerAssisted2.isAsMotionTrackerAssist = false;
                UpdateStatusTextAssist(motiontrackerAssistedIsBind2);
            }
            else
            {
                //motionTrackerSamplerAssisted2.gameObject.transform.SetParent(motionTrackerSampler.gameObject.transform,true);
                uIWriteConfig.SaveMotionTrackerTransformAssisted(motionTrackerSampler,motionTrackerSamplerAssisted2, motionTrackerSamplerAssisted1.id.value);
                motiontrackerAssistedIsBind2 = true;
                UpdateStatusTextAssist(motiontrackerAssistedIsBind2);
            }
        }
    }
    private void ReadJsonData(Vector3 jsonPos,Vector3 jsonRow,int ModelIndex, string jsonTrackerSN)
    {
      
        if (jsonTrackerSN.Equals(motionTrackerSampler.id))
        {
            isReadJson = true;
            jsonModelIndex = ModelIndex;
            SelectModelJson(jsonModelIndex, jsonRow);
            motionTrackerSampler.jsonOffestTrackerPostion = jsonPos;
            motionTrackerSampler.jsonOffestTrackerRotation = jsonRow- motionTrackerSampler.startRotation.eulerAngles;
            motionTrackerSampler.isReadJson = isReadJson;
            motiontrackerIsBind = true;
            StartCoroutine(JsonSetModelPartent());
            {
              offestInformation[0].text = ((int)(jsonPos.x * 100)).ToString();
              offestInformation[1].text = ((int)(jsonPos.y * 100)).ToString();
              offestInformation[2].text = ((int)(jsonPos.z * 100)).ToString();
              offestInformation[3].text = ((int)(jsonRow.x)).ToString();
              offestInformation[4].text = ((int)(jsonRow.y)).ToString();
              offestInformation[5].text = ((int)(jsonRow.z)).ToString();
            }
        }
        else
        {
            isReadJson = false;

            motionTrackerSampler.isReadJson = isReadJson;
        }
        UpdateStatusText();
       
    }
    IEnumerator JsonSetModelPartent()
    {
        yield return null;
        selectGameObjectModel.BindModelParent(motionTrackerSampler.transform);
    }
    public void SelectModelJson(int index, Vector3 jsonRow)
    {
        if (index == 0)
        {
            return;
        }
        else
        {
            motiontrackerIsBind = false;
            dropdownModel.value = index;
            selectModelIndex = index;
            gameObjectModelSetting.SetActive(true);
            template.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0);
            selectGameObjectModel.RemoveBinding();
            selectGameObjectModel.SelectModelJson(index, motionTrackerSampler.startPostion, motionTrackerSampler.startRotation, jsonRow);
            startTrackerPostion = motionTrackerSampler.transform.position;
            startTrackerRotation = motionTrackerSampler.transform.rotation.eulerAngles;
            UpdateStatusText();
            motiontrackerIsBind = true;
        }
        modelName = (SelectModelName)index;
    }
    private void ReadJsonDataAssist(Vector3 jsonPos, Vector3 jsonRow, string jsonTrackerSN)
    {

            if (motionTrackerSamplerAssisted1 != null && jsonTrackerSN.Equals(motionTrackerSamplerAssisted1.id))
            {
                motionTrackerSamplerAssisted1.isReadJson = true;
                SelectAssistedTracker(1);
                motionTrackerSamplerAssisted1.jsonOffestTrackerPostion = motionTrackerSampler.jsonOffestTrackerPostion-jsonPos;
                motionTrackerSamplerAssisted1.jsonOffestTrackerRotation = motionTrackerSampler.jsonOffestTrackerRotation- jsonRow;

            }
            if (motionTrackerSamplerAssisted2 != null&&jsonTrackerSN.Equals(motionTrackerSamplerAssisted2.id))
            {
                motionTrackerSamplerAssisted2.isReadJson = true;
                SelectAssistedTracker(2);
                motionTrackerSamplerAssisted2.jsonOffestTrackerPostion = motionTrackerSampler.jsonOffestTrackerPostion - jsonPos;
                motionTrackerSamplerAssisted2.jsonOffestTrackerRotation = motionTrackerSampler.jsonOffestTrackerRotation - jsonRow;
            }
            offestInformation[6].text = ((int)jsonPos.x * 100).ToString();
            offestInformation[7].text = ((int)(jsonPos.y * 100)).ToString();
            offestInformation[8].text = ((int)(jsonPos.z * 100)).ToString();
            offestInformation[9].text = ((int)(jsonRow.x)).ToString();
            offestInformation[10].text = ((int)(jsonRow.y)).ToString();
            offestInformation[11].text = ((int)(jsonRow.z)).ToString();
    }
    private void UpdateStatusText()
    {
        if (motiontrackerIsBind)
        {
            offestBtn.SetActive(true);
  
        }
        else
        {
            offestBtn.SetActive(false);
        }

        textStatus.text = LocalizationSettings.Instance.GetStringDatabase().GetTable("StringTable").GetEntry(motiontrackerIsBind ? "Manual Calibration" : "End calibration").GetLocalizedString();
    }
    private void UpdateStatusTextAssist(bool motiontrackerAssistIsBind)
    {
        textStatusAssist.text = LocalizationSettings.Instance.GetStringDatabase().GetTable("StringTable").GetEntry(motiontrackerAssistIsBind ? "Manual Calibration" : "End calibration").GetLocalizedString();
    }
    private void OnPosxPlus()
    {
        offestPos.x+=0.01f;
        offestInformation[0].text = ((int)((offestTrackerPostion.x + offestPos.x) * 100)).ToString();
        selectGameObjectModel.SetModelPostion(new Vector3(0.01f, 0, 0));
    }
    private void OnPosxMinus()
    {
        offestPos.x-= 0.01f;
        offestInformation[0].text = ((int)((offestTrackerPostion.x + offestPos.x) * 100)).ToString();
        selectGameObjectModel.SetModelPostion(new Vector3(-0.01f, 0, 0));
    }
    private void OnPosYPlus()
    {
        offestPos.y +=  0.01f;
        offestInformation[1].text = ((int)((offestTrackerPostion.y + offestPos.y) * 100)).ToString();
        selectGameObjectModel.SetModelPostion(new Vector3(0, 0.01f, 0));
    }
    private void OnPosYMinus()
    {
        offestPos.y -= 0.01f;
        offestInformation[1].text = ((int)((offestTrackerPostion.y + offestPos.y) * 100)).ToString();
        selectGameObjectModel.SetModelPostion(new Vector3(0, -0.01f, 0));
    }
    private void OnPosZPlus()
    {
        offestPos.z += 0.01f;
        offestInformation[2].text = ((int)((offestTrackerPostion.z + offestPos.z) * 100)).ToString();
        selectGameObjectModel.SetModelPostion(new Vector3(0,0,0.01f));
    }
    private void OnPosZMinus()
    {
        offestPos.z -= 0.01f;
        offestInformation[2].text = ((int)((offestTrackerPostion.z + offestPos.z) * 100)).ToString();
        selectGameObjectModel.SetModelPostion(new Vector3(0, 0,-0.01f));
    }
    private void OnRowxPlus()
    {
        offestRow.x += 1;
        offestInformation[3].text = ((int)(offestTrackerRotation.x + offestRow.x)).ToString();
        selectGameObjectModel.SetModelRotation(new Vector3(1, 0, 0));
    }
    private void OnRowxMinus()
    {
        offestRow.x -= 1;
        offestInformation[3].text = ((int)(offestTrackerRotation.x + offestRow.x)).ToString();
        selectGameObjectModel.SetModelRotation(new Vector3(-1, 0, 0));
    }
    private void OnRowYPlus()
    {
        offestRow.y += 1;
        offestInformation[4].text = ((int)(offestTrackerRotation.y + offestRow.y)).ToString();
        selectGameObjectModel.SetModelRotation(new Vector3(0, 1, 0));
    }
    private void OnRowYMinus()
    {
        offestRow.y -= 1;
        offestInformation[4].text = ((int)(offestTrackerRotation.y + offestRow.y)).ToString();
        selectGameObjectModel.SetModelRotation(new Vector3(0, -1, 0));
    }
    private void OnRowZPlus()
    {
        offestRow.z += 1;
        offestInformation[5].text = ((int)(offestTrackerRotation.z + offestRow.z)).ToString();
        selectGameObjectModel.SetModelRotation(new Vector3(0,0,1));
    }
    private void OnRowZMinus()
    {
        offestRow.z -= 1;
        offestInformation[5].text = ((int)(offestTrackerRotation.z + offestRow.z)).ToString();
        selectGameObjectModel.SetModelRotation(new Vector3(0, 0,-1));
    }
    private void OnConfidence(bool confidence)
    {
        if (confidence == bindPartent)
        {
            return;
        }
        else
        {
            bindPartent = confidence;
        }
        if (confidence&&(motiontrackerAssistedIsBind1||motiontrackerAssistedIsBind2))
        {
            Debug.Log("Laker" + bindPartent);
            selectGameObjectModel.BindModelParent(motionTrackerSampler.transform,offestPosSum,offestRowSum);
        }
        else
        {
            offestPosAssModel = offestPosAss - offestPosSum;
            Debug.Log("Laker" + motiontrackerAssistedIsBind1+"::"+ motiontrackerAssistedIsBind2);
            if (motiontrackerAssistedIsBind1)
            {
                selectGameObjectModel.BindModelAssParent(motionTrackerSamplerAssisted1.transform, offestPosAssModel, offestRowAss);
            }
            else if (motiontrackerAssistedIsBind2)
            {
                selectGameObjectModel.BindModelAssParent(motionTrackerSamplerAssisted2.transform, offestPosAssModel, offestRowAss);
            }
        }
    }
    private void OnTrackerCallBack(bool isTrackerAciton)
    {
        //if (isSelectPen)
        //{
        //    //selectGameObjectModel.Penparticle(isTrackerAciton);
        //    selectGameObjectModel.PenDrawing(isTrackerAciton);
        //}
    }
}
