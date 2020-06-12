import * as Yup from 'yup';

export interface ICrowdactionForm {
  crowdactionName: string;
  proposal: string;
  category: string;
  banner: any | null;
  target: string;
  startDate: string;
  endDate: string;
  tags: string;
  instagramUser: string;
  description: string;
  goal: string;
  image: any | null;
  imageDescription: string;
  comments: string;
  youtube: string;
}

export const initialValues: ICrowdactionForm = {
  crowdactionName: '',
  proposal: '',
  category: '',
  banner: null,
  target: '',
  startDate: '',
  tags: '',
  endDate: '',
  description: '',
  instagramUser: '',
  goal: '',
  image: null,
  imageDescription: '',
  comments: '',
  youtube: ''
};

const determineEndDateValidation = (startDate: any, schema: any) => {
  if (!startDate) {
    return;
  }

  const minDate = new Date(startDate) as Date;
  minDate.setDate(minDate.getDate() + 1);
  const maxDate = new Date(startDate);
  maxDate.setMonth(maxDate.getMonth() + 12);

  return schema
    .min(minDate, 'Please ensure your sign up ends after it starts :-)')
    .max(maxDate, 'The deadline must be within a year of the start date');
};

const determineImageDescriptionValidation = (image: any, schema: any) => {
  if (!image) {
    return;
  }

  return schema.required('Please provide a short description for the image');
}

var minimumDate = new Date();
var maximumDate = new Date();
maximumDate.setMonth(minimumDate.getMonth() + 12);

export const validations = Yup.object({
  crowdactionName: Yup.string()
    .required('Please write a clear, brief title')
    .max(50, 'Keep the name short, no more then 50 characters'),
  proposal: Yup.string()
    .required('Please describe your objective')
    .max(300, 'Best keep the objective short, no more then 300 characters'),
  category: Yup.string()
    .required('Select a category that most closely aligns with your crowdaction'),
  target: Yup.number()
    .required('Please choose the number of people you want to join')
    .moreThan(0, 'You can choose up to one million participants')
    .lessThan(1000001, 'Please choose no more then one million participants'),
  startDate: Yup.date()
    .required('Please set a launch date')
    .min(minimumDate, 'Please ensure the launch date starts somewhere in the near future')
    .max(maximumDate, 'Please ensure the launch date is within the next 12 months'),
  endDate: Yup.date()
    .required('Please enter the date when the sign-up closes')
    .when('startDate', determineEndDateValidation),
  hashtags: Yup.string()
    .max(30, 'Please keep the number of hashtags civil, no more then 30 characters')
    .matches(/^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$/, 'Don\'t use spaces or #, must contain a letter, can contain digits and underscores. Separate multiple tags with a colon \';\''),
  instagramUser: Yup.string(),
  description: Yup.string()
    .required('Give a succinct description of what you are gathering participants for')
    .max(10000, 'Please use no more then 10.000 characters'),
  goal: Yup.string()
    .required('Describe what you want achieve with your crowdaction')
    .max(10000, 'Please use no more then 10.000 characters'),
  imageDescription: Yup.string()
    .max(255, 'Please use no more then 255 characters')
    .when('image', determineImageDescriptionValidation),
  comments: Yup.string()
    .max(20000, 'Please use no more then 20.000 characters'),
  youtube: Yup.string()
    .matches(/^(https):\/\/www.youtube.com\/embed\/((?:\w|-){11}?)$/, 'Only YouTube links of the form https://www.youtube.com/embed/... are accepted.')
});
