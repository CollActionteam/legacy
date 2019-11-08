import React from "react";
import Layout from "../../components/Layout";
import { graphql } from "gatsby";
import styles from "./create.module.scss";
import { Section } from "../../components/Section";
import {
  Grid,
  Container,
  TextField,
  FormControl,
  InputLabel,
  createMuiTheme,
  MuiThemeProvider,
} from "@material-ui/core";
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker,
} from "@material-ui/pickers";
import { Button } from "../../components/Button";
import DateFnsUtils from "@date-io/date-fns";
import MUIRichTextEditor from "mui-rte";

export const query = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
  }
`;

const CreateProject = () => {
  const [selectedDate, setSelectedDate] = React.useState<Date | null>(
    new Date("2019-10-25")
  );

  const handleDateChange = (date: Date | null) => {
    setSelectedDate(date);
  };

  const richTextControls = [
    "bold",
    "italic",
    "underline",
    "numberList",
    "bulletList",
    "link",
  ];

  const defaultTheme = createMuiTheme();
  Object.assign(defaultTheme, {
    overrides: {
      MUIRichTextEditor: {
        root: {
          border: "1px solid var(--c-grey-d10)",
          borderRadius: "4px",
          marginTop: "-6px",
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

  return (
    <Layout>
      <form noValidate autoComplete="off">
        <div className={styles.projectBanner}>
          <div className={styles.uploadBanner}>
            <h3>Drop banner image here</h3>
            <span>Use pjg, png, gif or bmp. Max. 1MB</span>
          </div>
        </div>
        <Section className={`${styles.projectInfoBlock} ${styles.form}`}>
          <FormControl>
            <TextField
              className={styles.formControl}
              label="Project name"
              variant="outlined"
            ></TextField>
          </FormControl>
          <FormControl>
            <TextField
              className={styles.formControl}
              label="Category"
              variant="outlined"
            ></TextField>
          </FormControl>
          <FormControl>
            <TextField
              className={styles.formControl}
              label="Target"
              type="number"
              variant="outlined"
            ></TextField>
          </FormControl>
          <FormControl>
            <TextField
              className={styles.formControl}
              label="Proposal"
              multiline
              rows="4"
              variant="outlined"
              helperText={`e.g. "If X people commit to Y, we'll all do it together!"`}
            ></TextField>
          </FormControl>
        </Section>

        <MuiThemeProvider theme={defaultTheme}>
          <Container>
            <Grid container>
              <Grid item xs={12} md={5}>
                <Section className={styles.form}>
                  <InputLabel
                    htmlFor="description"
                    className={styles.formLabel}
                  >
                    Short description
                  </InputLabel>
                  <FormControl id="description">
                    <MUIRichTextEditor
                      label="E.g. reduce plastic waste and save our oceans!"
                      controls={richTextControls}
                    ></MUIRichTextEditor>
                  </FormControl>
                  <MuiPickersUtilsProvider utils={DateFnsUtils}>
                    <FormControl>
                      <KeyboardDatePicker
                        className={styles.formControl}
                        label="Sign up opens"
                        format="dd-MM-yyyy"
                        inputVariant="outlined"
                        value={selectedDate}
                        onChange={handleDateChange}
                      ></KeyboardDatePicker>
                    </FormControl>
                    <FormControl>
                      <KeyboardDatePicker
                        className={styles.formControl}
                        label="Sign up closes"
                        inputVariant="outlined"
                        format="dd-MM-yyyy"
                        value={selectedDate}
                        onChange={handleDateChange}
                      ></KeyboardDatePicker>
                    </FormControl>
                  </MuiPickersUtilsProvider>
                  <FormControl>
                    <TextField
                      className={styles.formControl}
                      label="Hashtags"
                      variant="outlined"
                      helperText="No #, seperate tags with ; e.g. tag1;tag2"
                    ></TextField>
                  </FormControl>
                </Section>
              </Grid>
              <Grid item xs={12} md={7}>
                <Section className={styles.form}>
                  <InputLabel htmlFor="goal" className={styles.formLabel}>
                    Goal/impact
                  </InputLabel>
                  <FormControl id="goal">
                    <MUIRichTextEditor
                      label="What is the problem you are trying to solve?"
                      controls={richTextControls}
                    ></MUIRichTextEditor>
                  </FormControl>
                  <InputLabel htmlFor="comments" className={styles.formLabel}>
                    Other comments
                  </InputLabel>
                  <FormControl id="comments">
                    <MUIRichTextEditor
                      label="E.g. background, process, FAQs, about the initiator"
                      controls={richTextControls}
                    ></MUIRichTextEditor>
                  </FormControl>
                  <FormControl>
                    <TextField
                      className={styles.formControl}
                      label="Descriptive image"
                      variant="outlined"
                      helperText="Will be replaced with file upload componentnpm"
                    ></TextField>
                  </FormControl>
                  <FormControl>
                    <TextField
                      className={styles.formControl}
                      label="YouTube Video Link"
                      variant="outlined"
                      helperText="Descriptive video, e.g. http://www.youtube.com/watch?v=-wtIMTCHWuI"
                    ></TextField>
                  </FormControl>
                </Section>
              </Grid>
              <Grid item xs={12}>
                <Section className={styles.submitProject}>
                  <Button type="submit">Submit</Button>
                </Section>
              </Grid>
            </Grid>
          </Container>
        </MuiThemeProvider>
      </form>
    </Layout>
  );
};

export default CreateProject;
