import React from "react";
import { graphql } from "gatsby";
import { Formik, Field, Form, FormikProps } from "formik";
import * as Yup from "yup";
import { TextField, Select } from "formik-material-ui";
import { FormControl, InputLabel, MenuItem, Grid } from "@material-ui/core";

import Layout from "../../components/Layout";
import { Section } from "../../components/Section";
import { RichTextEditorFormControl } from "../../components/RichTextEditorFormContol";
import { Button } from "../../components/Button";
import UploadBanner from "./upload-banner";
import UploadDescriptiveImage from "./upload-descriptive-image";

import styles from "./create.module.scss";
import gql from "graphql-tag";
import { useQuery, useMutation } from "react-apollo";
import Loader from "../../components/Loader";

export const query = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
  }
`;

const GET_CATEGORIES = gql`
  query {
    categories {
      id
      name
    }
  }
`;

export default () => {
  const minStartDate = new Date();
  const maxStartDate = new Date(minStartDate);
  maxStartDate.setMonth(new Date().getMonth() + 12);

  const setEndDateValidation = (startDate, schema) => {
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
  };

  const validate = async (props: FormikProps<any>) => {
    const errors = Object.keys(await props.validateForm());

    if (errors.length) {
      const el = document.getElementsByName(errors[0])[0];
      if (el) {
        el.scrollIntoView();
      }
    }
  };

  const { data: categoryResponse, loading } = useQuery(GET_CATEGORIES);

  const [createProject, { data: createProjectResponse }] = useMutation(gql`
    mutation project($project: NewProjectInputGraph!) {
      createProject(project: $project)
    }
  `);

  const commit = form => {
    createProject({
      variables: {
        project: {
          name: form.projectName,
          categoryId: form.category,
          target: form.target,
          proposal: form.proposal,
          description: form.description,
          start: form.startDate,
          end: form.endDate,
          goal: form.goal,
          tags: form.tags,
          creatorComments: form.comments,
          descriptionVideoLink: form.youtube,
        },
      },
    });

    console.log(createProjectResponse);
  };

  // TODO: banner image, descriptive image + description

  return (
    <Layout>
      {loading ? (
        <Loader />
      ) : (
        <Formik
          initialValues={{
            banner: null,
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
            image: null,
            imageDescription: "",
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
                .min(minStartDate, "Please ensure your sign up starts somewhere in the near future")
                .max(maxStartDate, "Please ensure your sign up starts within the next 12 months"),
              endDate: Yup.date()
                .required("Please enter the date until which people can sign up for the campaign")
                .when("startDate", setEndDateValidation),
              hashtags: Yup.string()
                .max(30, "Please keep the number of hashtags civil, no more then 30 characters")
                .matches(/^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$/, "Don't use spaces or #, must contain a letter, can contain digits and underscores. Seperate multiple tags with a colon ';'"),
              goal: Yup.string()
                .required("Describe what you hope to have achieved upon successful completion of your project")
                .max(10000, "Please use no more then 10.000 characters"),
              comments: Yup.string()
                .max(20000, "Please use no more then 20.000 characters"),
              imageDescription: Yup.string()
                .max(2000, "Keep it short, no more then 2000 characters"),
              youtube: Yup.string()
                .matches(/^(http|https):\/\/www.youtube.com\/watch\?v=((?:\w|-){11}?)$/, "Only YouTube links of the form http://www.youtube.com/watch?v= are accepted.")
              // tslint:enable: prettier
          })}
          onSubmit={(values, { setSubmitting }) => {
            commit(values);
            setSubmitting(false);
            // setTimeout(() => {
            //   console.log(values);
            //   setSubmitting(false);
            // }, 400);
          }}
        >
          {props => (
            <Form>
              <UploadBanner formik={props}></UploadBanner>

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
                    {categoryResponse
                      ? categoryResponse.categories.map(c => (
                          <MenuItem key={c.id} value={c.id}>
                            {c.name}
                          </MenuItem>
                        ))
                      : null}
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

              <Section>
                <Grid container>
                  <Grid item xs={12} md={5}>
                    <Section className={styles.form}>
                      <RichTextEditorFormControl
                        name="description"
                        label="Short description"
                        hint="E.g. reduce plastic waste and save our oceans!"
                        height="calc(var(--spacing) * 12.8)"
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
                    </Section>
                  </Grid>
                </Grid>

                <UploadDescriptiveImage formik={props}></UploadDescriptiveImage>

                <Grid container>
                  <Grid item xs={12} md={5}></Grid>
                  <Grid item xs={12} md={7}>
                    <Section className={styles.form}>
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

                <Grid container>
                  <Grid item xs={12}>
                    <Section className={styles.submitProject}>
                      <Button
                        type="submit"
                        disabled={props.isSubmitting}
                        onClick={() => validate(props)}
                      >
                        Submit
                      </Button>
                    </Section>
                  </Grid>
                </Grid>
              </Section>
            </Form>
          )}
        </Formik>
      )}
    </Layout>
  );
};
