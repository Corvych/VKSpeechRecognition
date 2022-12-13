# VKSpeechRecognition
A VK Speech Recognition API C# Library
## First Run
1. Get your app token by following [VK instruction](https://vk.com/voice-tech)
2. Copy repository .cs files into your C# project
3. Paste your token into "token":
```C#
private static string? token = "PASTE YOUR TOKEN HERE";
```
4. Call Methods in a specific order
```C#
SpeechRecognition.getServUrl();
SpeechRecognition.uplToVKServ("PATH\TO\YOUR\FILE");
SpeechRecognition.startSpeechRecog(SpeechRecognition.req);
SpeechRecognition.getTaskStat();
```
5. Get procession result in `SpeechRecognition.text`
