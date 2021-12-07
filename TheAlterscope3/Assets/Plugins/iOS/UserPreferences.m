bool GetBoolValue(const char* key)
{
    return [[NSUserDefaults standardUserDefaults] boolForKey:[NSString stringWithUTF8String: key]];
}
 
void SetBoolValue(const char* key, bool value)
{
    [[NSUserDefaults standardUserDefaults] setBool: value forKey:[NSString stringWithUTF8String: key]];
}