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

  static async uploadImage(file: File, description: string, resizeThreshold: number): Promise<number> {
    const body = new FormData();
    body.append('Image', file);
    body.append('ImageDescription', description);
    body.append('ImageResizeThreshold', resizeThreshold.toString());
    const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/image`, {
      method: 'POST',
      body,
      credentials: 'include'
    });
    const responseMessage = await response.json();
    if (response.ok) {
      return responseMessage.id as number;
    } else {
      throw responseMessage;
    }
  };
}
