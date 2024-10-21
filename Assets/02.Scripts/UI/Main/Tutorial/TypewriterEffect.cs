using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

[RequireComponent(typeof(TMP_Text))]
public class TypewriterEffect : MonoBehaviour
{
    private TMP_Text _sentence;

    private int _currentVisibleCharacterIndex;
    private Coroutine _typewriterCoroutine;
    private bool _readyForNewText = true;

    private WaitForSeconds _simpleDelay;
    private WaitForSeconds _interpunctuationDelay;

    [Header("----------- Typewriter Settings -----------")]
    [SerializeField]
    private float characterPerSecound = 20;
    [SerializeField]
    private float interpunctuationDelay = 0.5f;

    public bool CurrentlySkipping { get; private set; }
    private WaitForSeconds _skipDelay;

    [Header("----------- Skip options -----------")]
    [SerializeField]
    private bool quickSkip;
    [SerializeField]
    [Min(1)] private int skipSpeedup = 5;

    private WaitForSeconds _textboxFullEventDelay;
    [SerializeField]
    [Range(0.1f, 0.5f)] private float sendDoneDelay = 0.25f;

    public static event Action CompleteTextRevealed;
    public static event Action<char> CharacterRevealed;

    private void Awake()
    {
        _sentence = GetComponent<TMP_Text>();

        _simpleDelay = new WaitForSeconds(1/characterPerSecound);
        _interpunctuationDelay = new WaitForSeconds(interpunctuationDelay);

        _skipDelay = new WaitForSeconds(1 / (characterPerSecound * skipSpeedup));
        _textboxFullEventDelay = new WaitForSeconds(sendDoneDelay);
    }

    private void OnEnable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(PrepareForNewText);
    }

    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(PrepareForNewText);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (_sentence.maxVisibleCharacters != _sentence.textInfo.characterCount - 1)
            {
                Skip();
            }
        }
    }

    public void PrepareForNewText(Object obj)
    {
        if (!_readyForNewText)
        {
            return;
        }

        _readyForNewText = false;

        if (_typewriterCoroutine != null)
        {
            StopCoroutine(_typewriterCoroutine);
        }

        _sentence.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;

        _typewriterCoroutine = StartCoroutine(TypeWriter());
    }

    private IEnumerator TypeWriter()
    {
        TMP_TextInfo textInfo = _sentence.textInfo;

        while (_currentVisibleCharacterIndex < textInfo.characterCount + 1)
        {
            var lastCharacterIndex = textInfo.characterCount - 1;
            if (_currentVisibleCharacterIndex == lastCharacterIndex)
            {
                _sentence.maxVisibleCharacters++;
                yield return _textboxFullEventDelay;
                CompleteTextRevealed?.Invoke();
                _readyForNewText = true;
                yield break;
            }

            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;
            _sentence.maxVisibleCharacters++;

            if (!CurrentlySkipping &&
                (character == '?' || character == '.' || character == ',' || character == ':' ||
                character == ';' || character == '!' || character == '-'))
            {
                yield return _interpunctuationDelay;
            }
            else
            {
                yield return CurrentlySkipping ? _skipDelay : _simpleDelay;
            }

            CharacterRevealed?.Invoke(character);
            _currentVisibleCharacterIndex++;
        }
    }

    private void Skip()
    {
        if(CurrentlySkipping)
            return;

        CurrentlySkipping = true;

        if (!quickSkip)
        {
            StartCoroutine(SkipSpeedupReset());
            return;
        }

        StopCoroutine(_typewriterCoroutine);
        _sentence.maxVisibleCharacters = _sentence.textInfo.characterCount;
        _readyForNewText = true;
        CompleteTextRevealed?.Invoke();
    }

    private IEnumerator SkipSpeedupReset()
    {
        yield return new WaitUntil(() => _sentence.maxVisibleCharacters == _sentence.textInfo.characterCount - 1);
        CurrentlySkipping = false;
    }
}
