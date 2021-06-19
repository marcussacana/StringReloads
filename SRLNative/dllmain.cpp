#include "pch.h"
#include <cstddef>
#include <string>
#include <random>

std::wstring random_string(std::size_t length);

const wchar_t* PipeID = NULL;
wchar_t* PipeName;

HANDLE hPIPE = NULL;

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    switch (ul_reason_for_call)
    {
        case DLL_PROCESS_ATTACH:
        case DLL_THREAD_ATTACH:
        case DLL_THREAD_DETACH:
        case DLL_PROCESS_DETACH:
            break;
    }
    return TRUE;
}


bool NamedPipeExists(std::wstring pipePath)
{
    std::wstring pipeName = pipePath;
    if ((pipeName.size() < 10) ||
        (pipeName.compare(0, 9, L"\\\\.\\pipe\\") != 0) ||
        (pipeName.find(L'\\', 9) != std::string::npos))
    {
        // This can't be a pipe, so it also can't exist
        return false;
    }
    pipeName.erase(0, 9);

    WIN32_FIND_DATAW fd;
    DWORD dwErrCode;

    HANDLE hFind = FindFirstFileW(L"\\\\.\\pipe\\*", &fd);
    if (hFind == INVALID_HANDLE_VALUE)
    {
        dwErrCode = GetLastError();
    }
    else
    {
        do
        {
            if (pipeName == fd.cFileName)
            {
                FindClose(hFind);
                return true;
            }
        }         while (FindNextFileW(hFind, &fd));

        dwErrCode = GetLastError();
        FindClose(hFind);
    }

    if ((dwErrCode != ERROR_FILE_NOT_FOUND) &&
        (dwErrCode != ERROR_NO_MORE_FILES))
    {
        return false;
    }

    return false;
}

void Initialize();

void Initialize() {
    if (hPIPE != NULL)
        return;

    auto Rnd = random_string(10);
    PipeID = Rnd.c_str();
    PipeName = new wchar_t[255];
    
    auto PipePrefix = L"\\\\.\\pipe\\SRLPipe.";
    auto Len = lstrlenW(PipePrefix);

    memcpy(PipeName, PipePrefix, Len * 2);
    memcpy(PipeName + Len, PipeID, lstrlenW(PipeID) * 2);
    PipeName[Len + lstrlenW(PipeID)] = 0;

    auto CmdLinePrefix = L"rundll32 SRLWrapper.dll,Server ";

    wchar_t* CmdLine = new wchar_t[255];

    Len = lstrlenW(CmdLinePrefix);
    memcpy(CmdLine, CmdLinePrefix, Len * 2);
    memcpy(CmdLine + Len, PipeID, lstrlenW(PipeID) * 2);
    CmdLine[Len + lstrlenW(PipeID)] = 0;

    STARTUPINFO startup;
    PROCESS_INFORMATION process;
    memset(&startup, 0, sizeof(startup));
    startup.cb = sizeof(startup);
    memset(&process, 0, sizeof(process));

    wchar_t* CurDir = new wchar_t[2048];
    GetCurrentDirectoryW(2048, CurDir);
    
    if (!CreateProcessW(NULL, CmdLine, NULL, NULL, false, NULL, NULL, CurDir, &startup, &process))
    {
        auto Error = GetLastError();
        Initialize();
        return;
    }
    
    while (!NamedPipeExists(PipeName))
    {
        Sleep(100);
    }

    while (hPIPE == NULL)
        hPIPE = CreateFileW(PipeName, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
}

extern "C" __declspec(dllexport) void* Process(void* Ptr);

void* Process(void* Ptr) {
    Initialize();
    if (hPIPE == NULL)
        return Ptr;

    char* Mem = new char[4];
    Mem[0] = 1;

    DWORD Written = 0;
    WriteFile(hPIPE, Mem, 1, &Written, NULL);
    WriteFile(hPIPE, (LPCVOID)Ptr, strlen((char*)Ptr) + 1, &Written, NULL);

    if (Written == 0)
        return Ptr;

    DWORD Readed = 0;
    if (!ReadFile(hPIPE, Mem, 4, &Readed, NULL))
        return Ptr;

    if (Readed != 4) {
        delete[] Mem;
        return Ptr;
    }

    DWORD StrSize = *(DWORD*)Mem;

    if (StrSize == 0)
        return Ptr;

    delete[] Mem;

    unsigned char* Buffer = new unsigned char[StrSize];

    if (!ReadFile(hPIPE, Buffer, StrSize, &Readed, NULL)) {
        return Ptr;
    }

    if (Readed == 0)
        return Ptr;
   
    return Buffer;
}

extern "C" __declspec(dllexport) void* ProcessW(void* Ptr);

void* ProcessW(void* Ptr) {
    Initialize();
    if (hPIPE == NULL)
        return Ptr;

    char* Mem = new char[4];
    Mem[0] = 2;

    DWORD Written = 0;
    WriteFile(hPIPE, Mem, 1, &Written, NULL);
    WriteFile(hPIPE, (LPCVOID)Ptr, (lstrlenW((wchar_t*)Ptr) + 1) * 2, &Written, NULL);

    if (Written == 0)
        return Ptr;

    DWORD Readed = 0;
    if (!ReadFile(hPIPE, Mem, 4, &Readed, NULL))
        return Ptr;

    if (Readed != 4) {
        delete[] Mem;
        return Ptr;
    }

    DWORD StrSize = *(DWORD*)Mem;

    if (StrSize == 0)
        return Ptr;

    delete[] Mem;

    unsigned char* Buffer = new unsigned char[StrSize];

    if (!ReadFile(hPIPE, Buffer, StrSize, &Readed, NULL)) {
        return Ptr;
    }

    if (Readed == 0)
        return Ptr;

    return Buffer;
}

std::wstring random_string(std::size_t length)
{
    const std::wstring CHARACTERS = L"0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    std::random_device random_device;
    std::mt19937 generator(random_device());
    std::uniform_int_distribution<> distribution(0, CHARACTERS.size() - 1);

    std::wstring random_string;

    for (std::size_t i = 0; i < length; ++i)
    {
        random_string += CHARACTERS[distribution(generator)];
    }

    return random_string;
}