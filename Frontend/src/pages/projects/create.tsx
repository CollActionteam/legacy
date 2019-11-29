import React from "react";
import Layout from "../../components/Layout";
import { graphql } from "gatsby";
import styles from "./create.module.scss";
import { Section } from "../../components/Section";
import { Formik, Field, Form, FormikProps } from "formik";
import * as Yup from "yup";
import { TextField, Select } from "formik-material-ui";
import {
  FormControl,
  InputLabel,
  MenuItem,
  Container,
  Grid,
} from "@material-ui/core";
import { RichTextEditorFormControl } from "../../components/RichTextEditorFormContol";
import { Button } from "../../components/Button";
import UploadBanner from "./upload-banner";

export const query = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
  }
`;

export default class CreateComponent extends React.Component {
  minStartDate: Date;
  maxStartDate: Date;

  constructor(props: any) {
    super(props);

    this.minStartDate = new Date();
    this.maxStartDate = new Date(this.minStartDate);
    this.maxStartDate.setMonth(new Date().getMonth() + 12);
  }

  setEndDateValidation(startDate, schema) {
    if (!startDate) {
      return;
    }

    const minDate = new Date(startDate) as Date;
    minDate.setDate(minDate.getDate() + 1);
    const maxDate = new Date(startDate);
    maxDate.setMonth(maxDate.getMonth() + 12);

    return schema
      .min(minDate, "Please ensure your sign up ends after it starts :-)")
      .max(maxDate, "The deadline must be within a year of the start date");
  }

  async validate(props: FormikProps<any>) {
    const errors = Object.keys(await props.validateForm());

    if (errors.length) {
      const el = document.getElementsByName(errors[0])[0];
      if (el) {
        el.scrollIntoView();
      }
    }
  }

  render() {
    return (
      <Layout>
        <Formik
          initialValues={{
            projectName: "",
            category: 1,
            target: 0,
            proposal: "",
            description: "",
            startDate: "",
            endDate: "",
            hashtags: "",
            goal: "",
            comments: "",
            youtube: "",
          }}
          validateOnChange={false}
          validateOnBlur={true}
          validateOnMount={false}
          validationSchema={Yup.object({
            // tslint:disable: prettier
            projectName: Yup.string()
              .required("You must provide a name for your project")
              .max(50, "Keep the name short, no more then 50 characters"),
            target: Yup.number()
              .required("Please choose the target number of participants")
              .moreThan(0, "You can choose up to a maximum of one million participants as your target number")
              .lessThan(1000001, "You can choose up to a maximum of one million participants as your target number"),
            proposal: Yup.string()
              .required("Describe your proposal")
              .max(300, "Best keep your proposal short, no more then 300 characters"),
            description: Yup.string()
              .required("Give a succinct description of the issues your project is designed to address")
              .max(10000, "Please use no more then 10.000 characters"),
            startDate: Yup.date()
              .required("Please enter the date on which the campaign opens")
              .min(this.minStartDate, "Please ensure your sign up starts somewhere in the near future")
              .max(this.maxStartDate, "Please ensure your sign up starts within the next 12 months"),
            endDate: Yup.date()
              .required("Please enter the date until which people can sign up for the campaign")
              .when("startDate", this.setEndDateValidation),
            hashtags: Yup.string()
              .max(30, "Please keep the number of hashtags civil, no more then 30 characters")
              .matches(/^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$/, "Don't use spaces or #, must contain a letter, can contain digits and underscores. Seperate multiple tags with a colon ';'"),
            goal: Yup.string()
              .required("Describe what you hope to have achieved upon successful completion of your project")
              .max(10000, "Please use no more then 10.000 characters"),
            comments: Yup.string()
              .max(20000, "Please use no more then 20.000 characters"),
            youtube: Yup.string()
              .matches(/^(http|https):\/\/www.youtube.com\/watch\?v=((?:\w|-){11}?)$/, "Only YouTube links of the form http://www.youtube.com/watch?v= are accepted.")
            // tslint:enable: prettier
          })}
          onSubmit={(values, { setSubmitting }) => {
            setTimeout(() => {
              console.log(values);
              setSubmitting(false);
            }, 400);
          }}
        >
          {props => (
            <Form>
              <UploadBanner></UploadBanner>

              <Section className={`${styles.projectInfoBlock} ${styles.form}`}>
                <FormControl>
                  <Field
                    name="projectName"
                    label="Project name"
                    component={TextField}
                  ></Field>
                </FormControl>

                <FormControl>
                  <InputLabel htmlFor="category">Category</InputLabel>
                  <Field name="category" component={Select}>
                    <MenuItem value={1}>Environment</MenuItem>
                    <MenuItem value={2}>Community</MenuItem>
                    <MenuItem value={3}>Governance</MenuItem>
                    <MenuItem value={4}>Health</MenuItem>
                    <MenuItem value={5}>Consumption</MenuItem>
                    <MenuItem value={6}>Well-being</MenuItem>
                    <MenuItem value={7}>Other</MenuItem>
                  </Field>
                </FormControl>

                <FormControl>
                  <Field
                    name="target"
                    type="number"
                    label="Target"
                    component={TextField}
                  ></Field>
                </FormControl>

                <FormControl>
                  <Field
                    name="proposal"
                    label="Proposal"
                    multiline
                    rows="4"
                    helperText={`e.g. "If X people commit to Y, we'll all do it together!"`}
                    component={TextField}
                  ></Field>
                </FormControl>
              </Section>

              <Container>
                <Grid container>
                  <Grid item xs={12} md={5}>
                    <Section className={styles.form}>
                      <RichTextEditorFormControl
                        name="description"
                        label="Short description"
                        hint="E.g. reduce plastic waste and save our oceans!"
                        formik={props}
                      ></RichTextEditorFormControl>

                      <FormControl>
                        <Field
                          name="startDate"
                          type="date"
                          label="Sign up opens"
                          InputLabelProps={{
                            shrink: true,
                          }}
                          component={TextField}
                        ></Field>
                      </FormControl>

                      <FormControl>
                        <Field
                          name="endDate"
                          type="date"
                          label="Sign up closes"
                          InputLabelProps={{
                            shrink: true,
                          }}
                          component={TextField}
                        ></Field>
                      </FormControl>

                      <FormControl>
                        <Field
                          name="hashtags"
                          label="Hashtags"
                          helperText="No #, seperate tags with ; e.g. tag1;tag2"
                          component={TextField}
                        ></Field>
                      </FormControl>
                    </Section>
                  </Grid>

                  <Grid item xs={12} md={7}>
                    <Section className={styles.form}>
                      <RichTextEditorFormControl
                        name="goal"
                        label="Goal/impact"
                        hint="What is the problem you are trying to solve?"
                        formik={props}
                      ></RichTextEditorFormControl>

                      <RichTextEditorFormControl
                        name="comments"
                        label="Other comments"
                        hint="E.g. background, process, FAQs, about the initiator"
                        formik={props}
                      ></RichTextEditorFormControl>

                      <FormControl>
                        <Field
                          name="youtube"
                          label="YouTube Video Link"
                          helperText="Descriptive video, e.g. http://www.youtube.com/watch?v=-wtIMTCHWuI"
                          component={TextField}
                        ></Field>
                      </FormControl>
                    </Section>
                  </Grid>
                </Grid>
              </Container>

              <Container>
                <Grid container>
                  <Grid item xs={12}>
                    <Section className={styles.submitProject}>
                      <Button
                        type="submit"
                        disabled={props.isSubmitting}
                        onClick={() => this.validate(props)}
                      >
                        Submit
                      </Button>
                    </Section>
                  </Grid>
                </Grid>
              </Container>
            </Form>
          )}
        </Formik>
      </Layout>
    );
  }
}
