
export class Utility {
    public static convertWords(words: string) {
        let splitStr = words.split(' ');
        for (let i = 0; i < splitStr.length; i++) {
            splitStr[i] = splitStr[i].charAt(0).toUpperCase() + splitStr[i].substring(1);
        }
        return splitStr.join(' ');
    }

    public static isNullOrWhiteSpace(value: string | null) {
        if (value == null) return true;
        return value.replace(/^\s+|\s+$/g, "").length === 0;
    }
}

