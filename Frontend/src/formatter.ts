export default class Formatter {
  private static months = [
    'January',
    'February',
    'March',
    'April',
    'May',
    'June',
    'July',
    'August',
    'September',
    'October',
    'November',
    'December'
  ];
  private static days = [
    'Sunday',
    'Monday',
    'Tuesday',
    'Wednesday',
    'Thursday',
    'Friday',
    'Saturday'
  ];
  static date(date: Date) {
    return this.days[date.getDay()] + " " +
        date.getDate() + " " +
        this.months[date.getMonth()] + " " +
        date.getFullYear();
  }
  static time(date: Date) {
    return ("0" + date.getHours()).slice(-2) + ":" +
        ("0" + date.getMinutes()).slice(-2);
  }
  static timezone() {
    return Intl.DateTimeFormat().resolvedOptions().timeZone;
  }
  static largeNumber(number: number){
    return new Intl.NumberFormat('en-GB').format(number);
  }

}
