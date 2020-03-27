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
  height?: string;

  formik: FormikProps<any>;
  className?: string;
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
            height: this.props.height || "calc(var(--spacing) * 20)",
            overflow: "scroll",
          },
        },
      },
    });
  }

  hasError() {
    return (
      this.props.formik.touched[this.props.name] !== undefined &&
      this.props.formik.errors[this.props.name] !== undefined
    );
  }

  render() {
    return (
      <MuiThemeProvider theme={this.defaultTheme}>
        <FormControl id={this.props.name} fullWidth className={this.props.className}>
          <InputLabel
            error={this.hasError()}
            htmlFor={this.props.name}
            className={styles.label}
            shrink
          >
            {this.props.label}
          </InputLabel>
          <MUIRichTextEditor
            error={this.hasError()}
            label={this.props.hint}
            controls={this.richTextControls}
            onChange={(state: any)=> {
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
