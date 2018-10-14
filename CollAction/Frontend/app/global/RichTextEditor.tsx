import * as React from "react";
import renderComponentIf from "./renderComponentIf";
const ReactQuill = require("react-quill");

interface ICollActionEditorProps {
  formInputId: string;
  hint: string;
  simple?: boolean;
}

interface ICollActionEditorState {
  content?: string;
}

export default class RichTextEditor extends React.Component<ICollActionEditorProps, ICollActionEditorState> {

  private HREF_REGEX = /href=\"(?!http:\/\/|https:\/\/)([^\"]*)(\")/ig;

  constructor(props?: ICollActionEditorProps, context?: any) {
    super(props, context);

    this.state = {
      content: ""
    };
    this.handleChange = this.handleChange.bind(this);

    this.modules = {
      toolbar: [
        ["bold", "italic", "underline"], [{"list": "ordered"}, {"list": "bullet"}], ["link"]
      ]
    };

    this.formats = [
      "bold", "italic", "underline", "list", "link"
    ];
  }

  modules: any;
  formats: any;

  componentWillMount() {
    const input = document.getElementById(this.props.formInputId) as HTMLInputElement;
    if (input) {
      this.setState({ content: input.value });
    }
  }

  handleChange(value) {
    value = this.fixLinks(value);

    this.setState({ content: value });

    const input = document.getElementById(this.props.formInputId) as HTMLInputElement;
    if (input) {
      input.value = this.state.content;
    }
  }

  private fixLinks(value: string): string {
    let link;

    while ((link = this.HREF_REGEX.exec(value)) !== null) {
      let url = link[1];
      value = value.replace(url, "http://" + url);
    }

    return value;
  }

  render() {
    return (
      <ReactQuill
        theme="snow"
        value={ this.state.content }
        placeholder={ this.props.hint }
        modules={ this.modules }
        formats={ this.formats }
        onChange={ this.handleChange }></ReactQuill>
    );
  }
}

renderComponentIf(
  <RichTextEditor
    formInputId="Message"
    hint="Hallo {firstname}!, Hier is onze project update..." />,
  document.getElementById("project-email-message")
);

renderComponentIf(
  <RichTextEditor
    formInputId="Description"
    hint="E.g Reduceer plastic afval en red onze oceanen!" />,
  document.getElementById("create-project-description")
);

renderComponentIf(
  <RichTextEditor
    formInputId="Goal"
    hint="Max 1000 karakters" />,
  document.getElementById("create-project-goal")
);

renderComponentIf(
  <RichTextEditor
    formInputId="CreatorComments"
    hint="E.g Achtergrond, process, FAQs, over de initiatiefnemer" />,
  document.getElementById("create-project-creatorcomments")
);
