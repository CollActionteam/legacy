export default class Utils {
  static formatCategory(category: string) {
    return (
      category.charAt(0).toUpperCase() +
      category
        .substring(1)
        .replace("_", "-")
        .toLowerCase()
    );
  }

  static async uploadImage(file: File, description: string): Promise<any> {
    const body = new FormData();
    body.append('Image', file);
    body.append('ImageDescription', description);
    const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/image`, {
      method: 'POST',
      body,
      credentials: 'include'
    });
    return response.json();
  };
}
