using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UserManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;
    public TMP_Text currentUserText;
    public TMP_Text xpText;
    public TMP_Text timerDefaultXp;
    public TMP_Text alertText;

    public Button saveButton;
    public Button loadButton;
    public Button switchButton;
    public Button deleteButton;

    public TMP_Dropdown saveSlotDropdown;
    public GameObject addAkunPanel;
    public GameObject currentAkunPanel;

    public UserAccount currentUser;
    private Dictionary<string, UserAccount> accounts = new Dictionary<string, UserAccount>();

    private float xpTimer = 0f;

    void Start()
    {
        LoadAccounts();
        UpdateDropdownOptions();
        saveSlotDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        saveButton.onClick.AddListener(SaveAccount);
        loadButton.onClick.AddListener(LoadAccount);
        switchButton.onClick.AddListener(SwitchAccount);
        deleteButton.onClick.AddListener(DeleteAccount);

        if (currentUser == null || string.IsNullOrEmpty(currentUser.username))
        {
            currentAkunPanel.SetActive(false);
            addAkunPanel.SetActive(true);
        }
        else
        {
            currentAkunPanel.SetActive(true);
            addAkunPanel.SetActive(false);
            UpdateUI();
        }

        StartCoroutine(XPGainTimer());
    }

    void UpdateDropdownOptions()
    {
        saveSlotDropdown.ClearOptions();
        List<string> options = new List<string> { "ADD..." };

        foreach (var account in accounts)
        {
            options.Add(account.Key);
        }

        saveSlotDropdown.AddOptions(options);
    }

    public void OnDropdownValueChanged(int value)
    {
        if (saveSlotDropdown.options[value].text == "ADD...")
        {
            usernameInput.gameObject.SetActive(true);
            passwordInput.gameObject.SetActive(true);
            passwordInput.interactable = true;
            saveButton.gameObject.SetActive(true);
            loadButton.gameObject.SetActive(true);
        }
        else
        {
            usernameInput.gameObject.SetActive(false);
            passwordInput.gameObject.SetActive(true);
            saveButton.gameObject.SetActive(true);
            loadButton.gameObject.SetActive(true);
        }
    }

    public void SaveAccount()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            StartCoroutine(ShowAlert("Username dan Password tidak boleh kosong!"));
            return;
        }

        if (!accounts.ContainsKey(username))
        {
            UserAccount newUser = new UserAccount(username, password, 0);
            newUser.xp += 1;
            accounts.Add(username, newUser);
            GameSaveManager.SaveGame(newUser, 1);
            SaveUsers();

            StartCoroutine(ShowAlert("Akun berhasil disimpan!"));
            UpdateDropdownOptions();
            ResetInputFields();
        }
        else
        {
            StartCoroutine(ShowAlert("Username sudah terdaftar!"));
        }
    }

    public void LoadAccount()
    {
        string selectedUsername = saveSlotDropdown.options[saveSlotDropdown.value].text;
        string password = passwordInput.text;

        Debug.Log("Mencoba memuat akun: " + selectedUsername);

        if (!accounts.ContainsKey(selectedUsername))
        {
            Debug.LogError("Akun tidak ditemukan di dictionary!");
            StartCoroutine(ShowAlert("Akun tidak ditemukan!"));
            return;
        }

        UserAccount user = accounts[selectedUsername];

        if (user.password == password)
        {
            Debug.Log("Akun ditemukan dan password cocok!");
            currentUser = user;
            currentUserText.text = currentUser.username;
            addAkunPanel.SetActive(false);
            currentAkunPanel.SetActive(true);
            UpdateUI();
            StartCoroutine(ShowAlert("Akun berhasil dimuat!"));
        }
        else
        {
            Debug.LogError("Password salah!");
            StartCoroutine(ShowAlert("Password salah!"));
        }
    }


    void LoadAccounts()
    {
        accounts.Clear();
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path, "*_slot1.bin");

        Debug.Log("Memeriksa file akun di folder: " + path);

        foreach (string file in files)
        {
            Debug.Log("Ditemukan file: " + file);
            string username = Path.GetFileNameWithoutExtension(file).Replace("_slot1", "");
            UserAccount account = GameSaveManager.LoadGame(username, 1);

            if (account != null)
            {
                accounts[username] = account;
                Debug.Log("Akun dimuat: " + username);
            }
            else
            {
                Debug.LogError("Gagal memuat akun dari file: " + file);
            }
        }
    }
    public void SwitchAccount()
    {
        currentUser = null;
        currentUserText.text = "Tidak ada akun";
        addAkunPanel.SetActive(true);
        currentAkunPanel.SetActive(false);

        ResetInputFields();
        StartCoroutine(ShowAlert("Akun telah diganti!"));
    }
    public void DeleteAccount()
    {
        if (currentUser == null) return;

        string username = currentUser.username;
        string filePath = Path.Combine(Application.persistentDataPath, $"{username}_slot1.bin");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            accounts.Remove(username);
            ResetInputFields();
            StartCoroutine(ShowAlert($"Akun {username} telah dihapus!"));
        }

        SwitchAccount();
    }
    void SaveUsers()
    {
        string accountList = string.Join(";", accounts.Keys);
        PlayerPrefs.SetString("SavedAccounts", accountList);
        PlayerPrefs.Save();
    }
    void ResetInputFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
    }


    public IEnumerator ShowAlert(string message)
    {
        alertText.text = message;
        yield return new WaitForSeconds(3f);
        alertText.text = "";
    }

    IEnumerator XPGainTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(180);
            if (currentUser != null)
            {
                currentUser.xp += 1;
                SaveAccount();
                UpdateUI();
            }
        }
    }

    public void UpdateUI()
    {
        if (currentUser != null)
        {
            xpText.text = "XP: " + currentUser.xp;
        }
    }
}

[System.Serializable]
public class UserAccount
{
    public string username;
    public string password;
    public int xp;

    public UserAccount(string username, string password, int xp)
    {
        this.username = username;
        this.password = password;
        this.xp = xp;
    }
}