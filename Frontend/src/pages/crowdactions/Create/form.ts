import * as Yup from 'yup';
import { isDate, parse, addDays, startOfDay, addMonths } from 'date-fns';

export interface ICrowdactionForm {
  crowdactionName: string;
  proposal: string;
  category: string;
  secondCategory: string;
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

export const initialValues: ICrowdactionForm = {
  crowdactionName: '',
  proposal: '',
  category: '',
  secondCategory: '',
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

const today = startOfDay(new Date());

const determineImageDescriptionValidation = (image: any, schema: any) => {
  if (!image) {
    return;
  }

  return schema.required('Please provide a short description for the image');
}

const transformToDate = (value: Date, originalValue: string | Date) => 
  isDate(originalValue) ? originalValue : parseDate(originalValue as string);

export const parseDate = (value: string) =>
  parse(value, 'd/M/yyyy', new Date());

export const validations = Yup.object({
  crowdactionName: Yup.string()
    .required('Please write a clear, brief title')
    .max(50, 'Keep the name short, no more then 50 characters'),
  proposal: Yup.string()
    .required('Please describe your objective')
    .max(300, 'Best keep the objective short, no more then 300 characters'),
  category: Yup.string()
    .required('Select a category that most closely aligns with your crowdaction'),
  secondCategory: Yup.string()
                     .when("category", {
                         is: val => val?.length > 0,
                         then: Yup
                             .string()
                             .notOneOf([Yup.ref("category")], "The two categories can't be the same")
                          }),
  target: Yup.number()
    .required('Please choose the number of people you want to join')
    .moreThan(0, 'You can choose up to one million participants')
    .lessThan(1000001, 'Please choose no more then one million participants'),
  startDate: Yup.date()
    .transform(transformToDate).typeError('Please enter a valid date, using the format d/M/yyyy')
    .required('Please enter a launch date')
    .min(addDays(today, 1), 'Please ensure the launch date starts somewhere in the near future')
    .max(addMonths(today, 12), 'Please ensure the launch date is within the next 12 months'),
  endDate: Yup.date()
    .transform(transformToDate).typeError('Please enter a valid date, using the format d/M/yyyy')
    .required('Please enter an end date')
    .when('startDate', (started: Date, yup: any) => started && yup.min(addDays(started, 1), 'Please ensure your sign up ends after it starts :-)'))
    .when('startDate', (started: Date, yup: any) => started && yup.max(addMonths(started, 12), 'The deadline must be within a year of the start date')),
  tags: Yup.string()
    .max(300, 'Please keep the number of hashtags civil, no more then 300 characters')
    .matches(/^([A-Za-z][A-Za-z0-9_-]{0,29})+(;[A-Za-z][A-Za-z0-9_-]{0,29})*$/, 'Tags must be between one and thirty characters, start with a letter, and only contain letters, numbers, underscores or dashes. Seperate tags with a colon \';\''),
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
