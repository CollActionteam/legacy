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

  static async uploadImage(file: File, description: string, resizeThreshold: number): Promise<any> {
    const body = new FormData();
    body.append('Image', file);
    body.append('ImageDescription', description);
    body.append('ImageResizeThreshold', resizeThreshold.toString());
    const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/image`, {
      method: 'POST',
      body,
      credentials: 'include'
    });
    return response.json();
  };

  static parseProjectEndDate(date: string) {
    let dateParts = date.split("-");

    return new Date(
        Number(dateParts[0]),
        Number(dateParts[1]),
        Number(dateParts[2]),
        23 - new Date().getTimezoneOffset() / 60,
        59,
        59,
    );
  }

}
