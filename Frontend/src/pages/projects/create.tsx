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
  Theme,
} from "@material-ui/core";
import {
  MuiPickersUtilsProvider,
  KeyboardDatePicker,
} from "@material-ui/pickers";
import { Button } from "../../components/Button";
import DateFnsUtils from "@date-io/date-fns";
import MUIRichTextEditor from "mui-rte";
import { stateToMarkdown } from "draft-js-export-markdown";

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
  state = {
    name: "The project",
    category: "",
    number: 0,
    proposal: "",
    description: "",
    startDate: new Date(),
    endDate: new Date(),
    hashtags: "",
    goal: "",
    comments: "",
    imageDescription: "",
    youtube: "",
  };
  private defaultTheme: Theme;

  constructor(props: any) {
    super(props);

    this.defaultTheme = createMuiTheme();
    Object.assign(this.defaultTheme, {
      overrides: {
        MUIRichTextEditor: {
          root: {
            border: "1px solid var(--c-grey-d20)",
            borderRadius: "4px",
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

  handleDateChange(date: Date | null) {
    console.log(date);
  }

  handleInputChange = (event: any) => {
    const target = event.target;
    const value = target.value;
    const name = target.name;
    this.setState({
      [name]: value,
    });
    // tslint:disable-next-line: prettier
  }

  handleDescriptionChange = (event: any) => {
    // TODO: can't update state here??
    const value = stateToMarkdown(event.getCurrentContent());
    // tslint:disable-next-line: prettier
  }

  handleGoalChange = (state: any) => {
    const value = stateToMarkdown(state.getCurrentContent());
    this.setState({
      goal: value,
    });
    // tslint:disable-next-line: prettier
  }

  handleCommentsChange = (state: any) => {
    const value = stateToMarkdown(state.getCurrentContent());
    this.setState({
      description: value,
    });
    // tslint:disable-next-line: prettier
  }

  submit = (event: any) => {
    event.preventDefault();
    console.log(this.state);
    // tslint:disable-next-line: prettier
  }

  render() {
    const richTextControls = [
      "bold",
      "italic",
      "underline",
      "numberList",
      "bulletList",
      "link",
    ];

    return (
      <Layout>
        <form noValidate autoComplete="off" onSubmit={this.submit}>
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
                name="name"
                label="Project name"
                variant="outlined"
                value={this.state.name}
                onChange={this.handleInputChange}
              ></TextField>
            </FormControl>
            <FormControl>
              <TextField
                className={styles.formControl}
                name="category"
                label="Category"
                variant="outlined"
                value={this.state.category}
                onChange={this.handleInputChange}
              ></TextField>
            </FormControl>
            <FormControl>
              <TextField
                className={styles.formControl}
                name="number"
                label="Target"
                type="number"
                variant="outlined"
                value={this.state.number}
                onChange={this.handleInputChange}
              ></TextField>
            </FormControl>
            <FormControl>
              <TextField
                className={styles.formControl}
                name="proposal"
                label="Proposal"
                multiline
                rows="4"
                variant="outlined"
                helperText={`e.g. "If X people commit to Y, we'll all do it together!"`}
                value={this.state.proposal}
                onChange={this.handleInputChange}
              ></TextField>
            </FormControl>
          </Section>

          <MuiThemeProvider theme={this.defaultTheme}>
            <Container>
              <Grid container>
                <Grid item xs={12} md={5}>
                  <Section className={styles.form}>
                    <InputLabel
                      htmlFor="description"
                      className={styles.rteFormLabel}
                    >
                      Short description
                    </InputLabel>
                    <FormControl id="description">
                      <MUIRichTextEditor
                        label="E.g. reduce plastic waste and save our oceans!"
                        controls={richTextControls}
                        value={this.state.description}
                        onChange={this.handleDescriptionChange}
                      ></MUIRichTextEditor>
                    </FormControl>
                    <MuiPickersUtilsProvider utils={DateFnsUtils}>
                      <FormControl>
                        <KeyboardDatePicker
                          className={styles.formControl}
                          label="Sign up opens"
                          format="dd-MM-yyyy"
                          inputVariant="outlined"
                          value={this.state.startDate}
                          onChange={this.handleDateChange}
                        ></KeyboardDatePicker>
                      </FormControl>
                      <FormControl>
                        <KeyboardDatePicker
                          className={styles.formControl}
                          label="Sign up closes"
                          inputVariant="outlined"
                          format="dd-MM-yyyy"
                          value={this.state.endDate}
                          onChange={this.handleDateChange}
                        ></KeyboardDatePicker>
                      </FormControl>
                    </MuiPickersUtilsProvider>
                    <FormControl>
                      <TextField
                        className={styles.formControl}
                        label="Hashtags"
                        variant="outlined"
                        helperText="No #, seperate tags with ; e.g. tag1;tag2"
                        value={this.state.hashtags}
                        onChange={this.handleInputChange}
                      ></TextField>
                    </FormControl>
                  </Section>
                </Grid>
                <Grid item xs={12} md={7}>
                  <Section className={styles.form}>
                    <InputLabel htmlFor="goal" className={styles.rteFormLabel}>
                      Goal/impact
                    </InputLabel>
                    <FormControl id="goal">
                      <MUIRichTextEditor
                        label="What is the problem you are trying to solve?"
                        controls={richTextControls}
                        value={this.state.goal}
                        onChange={this.handleGoalChange}
                      ></MUIRichTextEditor>
                    </FormControl>
                    <InputLabel
                      htmlFor="comments"
                      className={styles.rteFormLabel}
                    >
                      Other comments
                    </InputLabel>
                    <FormControl id="comments">
                      <MUIRichTextEditor
                        label="E.g. background, process, FAQs, about the initiator"
                        controls={richTextControls}
                        value={this.state.comments}
                        onChange={this.handleCommentsChange}
                      ></MUIRichTextEditor>
                    </FormControl>
                    <FormControl>
                      <TextField
                        className={styles.formControl}
                        label="Descriptive image"
                        variant="outlined"
                        helperText="Will be replaced with file upload componentnpm"
                        value={this.state.imageDescription}
                        onChange={this.handleInputChange}
                      ></TextField>
                    </FormControl>
                    <FormControl>
                      <TextField
                        className={styles.formControl}
                        label="YouTube Video Link"
                        variant="outlined"
                        helperText="Descriptive video, e.g. http://www.youtube.com/watch?v=-wtIMTCHWuI"
                        value={this.state.youtube}
                        onChange={this.handleInputChange}
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
  }
}
