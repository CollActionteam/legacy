import React from "react";
import {
  Theme,
  createMuiTheme,
  InputLabel,
  FormControl,
  MuiThemeProvider,
} from "@material-ui/core";
import MUIRichTextEditor from "mui-rte";
import { stateToHTML } from "draft-js-export-html";
import { Field, ErrorMessage, FormikProps } from "formik";

import styles from "./style.module.scss";

export interface IRichTextEditorProps {
  name: string;
  label: string;
  hint?: string;

  formik: FormikProps<any>;
}

export class RichTextEditorFormControl extends React.Component<
  IRichTextEditorProps
> {
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

  hasError = () => this.props.formik.errors[this.props.name] !== undefined;

  render() {
    return (
      <MuiThemeProvider theme={this.defaultTheme}>
        <InputLabel
          error={this.hasError()}
          htmlFor={this.props.name}
          className={styles.label}
        >
          {this.props.label}
        </InputLabel>
        <FormControl id={this.props.name}>
          <MUIRichTextEditor
            error={this.hasError()}
            label={this.props.hint}
            controls={this.richTextControls}
            onChange={state => {
              const content = state.getCurrentContent();
              this.props.formik.setFieldValue(
                this.props.name,
                content.hasText() ? stateToHTML(content) : "",
                true
              );
            }}
          ></MUIRichTextEditor>
          <Field name={this.props.name} type="hidden"></Field>
          <p className="MuiFormHelperText-root Mui-error">
            <ErrorMessage name={this.props.name} />
          </p>
        </FormControl>
      </MuiThemeProvider>
    );
  }
}
