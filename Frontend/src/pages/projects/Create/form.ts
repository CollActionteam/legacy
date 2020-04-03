import * as Yup from 'yup';

export interface IProjectForm {
  projectName: string;
  proposal: string;
  category: string;
  banner: any | null;
  target: string;
  startDate: string;
  endDate: string;
  tags: string;
  description: string;
  goal: string;
  image: any | null;
  imageDescription: string;
  comments: string;
  youtube: string;
}

export const initialValues: IProjectForm = {
  projectName: '',
  proposal: '',
  category: '',
  banner: null,
  target: '',
  startDate: '',
  tags: '',
  endDate: '',
  description: '',
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

var minimumDate = new Date();
var maximumDate = new Date();
maximumDate.setMonth(minimumDate.getMonth() + 12);

export const validations = Yup.object({
  projectName: Yup.string()
    .required('You must provide a name for your project')
    .max(50, 'Keep the name short, no more then 50 characters'),
  proposal: Yup.string()
    .required('Describe your proposal')
    .max(300, 'Best keep your proposal short, no more then 300 characters'),
  category: Yup.string()
    .required('Select a category for your project'),
  target: Yup.number()
    .required('Please choose the target number of participants')
    .moreThan(0, 'You can choose up to a maximum of one million participants as your target number')
    .lessThan(1000001, 'You can choose up to a maximum of one million participants as your target number'),
  startDate: Yup.date()
    .required('Please enter the date on which the campaign opens')
    .min(minimumDate, 'Please ensure your sign up starts somewhere in the near future')
    .max(maximumDate, 'Please ensure your sign up starts within the next 12 months'),
  endDate: Yup.date()
    .required('Please enter the date until which people can sign up for the campaign')
    .when('startDate', determineEndDateValidation),
  hashtags: Yup.string()
    .max(30, 'Please keep the number of hashtags civil, no more then 30 characters')
    .matches(/^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$/, 'Don\'t use spaces or #, must contain a letter, can contain digits and underscores. Seperate multiple tags with a colon \';\''),
  description: Yup.string()
    .required('Give a succinct description of the issues your project is designed to address')
    .max(10000, 'Please use no more then 10.000 characters'),
  goal: Yup.string()
    .required('Describe what you hope to have achieved upon successful completion of your project')
    .max(10000, 'Please use no more then 10.000 characters'),
  imageDescription: Yup.string()
    .max(255, 'Please use no more then 255 characters'),
  comments: Yup.string()
    .max(20000, 'Please use no more then 20.000 characters'),
  youtube: Yup.string()
    .matches(/^(https):\/\/www.youtube.com\/embed\/((?:\w|-){11}?)$/, 'Only YouTube links of the form https://www.youtube.com/embed/... are accepted.')
});
