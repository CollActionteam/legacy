import React from "react";
import Layout from "../../components/Layout";
import { graphql } from "gatsby";
import styles from "./create.module.scss";
import { Section } from "../../components/Section";
import { Formik, Field, Form } from "formik";
import * as Yup from "yup";
import { TextField, Select } from "formik-material-ui";
import {
  FormControl,
  InputLabel,
  MenuItem,
  Container,
  Grid,
  Button,
  Theme,
  createMuiTheme,
  MuiThemeProvider,
} from "@material-ui/core";
import MUIRichTextEditor from "mui-rte";
import { stateToHTML } from "draft-js-export-html";

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
  private defaultTheme: Theme;
  private richTextControls = [
    "bold",
    "italic",
    "underline",
    "numberList",
    "bulletList",
    "link",
  ];

  constructor(props: any) {
    super(props);

    this.defaultTheme = createMuiTheme();
    Object.assign(this.defaultTheme, {
      overrides: {
        MUIRichTextEditor: {
          root: {
            borderBottom: "1px solid var(--c-grey-d20)",
          },
          editorContainer: {
            padding: "var(--spacing-sm)",
            width: "unset",
            height: "120px",
            overflow: "scroll",
          },
        },
      },
    });
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
              .required("You must provide a name for your project"),
            target: Yup.number()
              .required("Please choose the target number of participants")
              .moreThan(0, "You can choose up to a maximum of one million participants as your target number")
              .lessThan(1000001, "You can choose up to a maximum of one million participants as your target number"),
            proposal: Yup.string()
              .required("Describe your proposal")
              .max(300, "Best keep your proposal short, no more then 300 characters"),
            startDate: Yup.date()
              .required("Please enter the date on which the campaign opens"),
            endDate: Yup.date()
              .required("Please enter the date until which people can sign up for the campaign"),
            hashtags: Yup.string()
              .max(30, "Please keep the number of hashtags civil, no more then 30 characters")
              .matches(/^[a-zA-Z_0-9]+(;[a-zA-Z_0-9]+)*$/, "Don't use spaces or #, must contain a letter, can contain digits and underscores. Seperate multiple tags with a colon ';'"),
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
              <div className={styles.projectBanner}>
                <div className={styles.uploadBanner}>
                  <h3>Drop banner image here</h3>
                  <span>Use jpg, png, gif or bmp. Max. 1MB</span>
                </div>
              </div>

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

              <MuiThemeProvider theme={this.defaultTheme}>
                <Container>
                  <Grid container>
                    <Grid item xs={12} md={5}>
                      <Section className={styles.form}>
                        <InputLabel
                          htmlFor="description"
                          className={styles.rteLabel}
                        >
                          Short description
                        </InputLabel>
                        <FormControl id="description">
                          <MUIRichTextEditor
                            label="E.g. reduce plastic waste and save our oceans!"
                            controls={this.richTextControls}
                            onChange={state =>
                              props.setFieldValue(
                                "description",
                                stateToHTML(state.getCurrentContent())
                              )
                            }
                          ></MUIRichTextEditor>
                          <Field name="description" type="hidden"></Field>
                        </FormControl>

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
                        <InputLabel htmlFor="goal" className={styles.rteLabel}>
                          Goal/impact
                        </InputLabel>
                        <FormControl id="goal">
                          <MUIRichTextEditor
                            label="What is the problem you are trying to solve?"
                            controls={this.richTextControls}
                            onChange={state =>
                              props.setFieldValue(
                                "goal",
                                stateToHTML(state.getCurrentContent())
                              )
                            }
                          ></MUIRichTextEditor>
                          <Field name="goal" type="hidden"></Field>
                        </FormControl>

                        <InputLabel
                          htmlFor="comments"
                          className={styles.rteLabel}
                        >
                          Other comments
                        </InputLabel>
                        <FormControl id="comments">
                          <MUIRichTextEditor
                            label="E.g. background, process, FAQs, about the initiator"
                            controls={this.richTextControls}
                            onChange={state =>
                              props.setFieldValue(
                                "comments",
                                stateToHTML(state.getCurrentContent())
                              )
                            }
                          ></MUIRichTextEditor>
                          <Field name="comments" type="hidden"></Field>
                        </FormControl>

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
              </MuiThemeProvider>

              <Container>
                <Grid container>
                  <Grid item xs={12}>
                    <Section className={styles.submitProject}>
                      <Button type="submit">Submit</Button>
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
