export default class Utils {
    static formatCategory(category: string)
    {
        return category.charAt(0).toUpperCase() + category.substring(1).replace("_", " ").toLowerCase();
    }
}