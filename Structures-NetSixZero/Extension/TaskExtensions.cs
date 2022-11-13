namespace Structures.NetSixZero.Extension; 

public static class TaskExtensions {
    public static async Task WaitUntil(Func<bool> condition, int sleepTimeMs = 10) {
        var result = condition();
        while (!result) {
            await Task.Delay(sleepTimeMs);
            result = condition();
        }
    }
}