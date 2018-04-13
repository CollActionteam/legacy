import * as React from "react";
import * as ReactQuill from "react-quill";
import renderComponentIf from "./renderComponentIf";

interface ICollActionEditorProps {
  formInputId: string;
  hint: string;
  simple?: boolean;
}

interface ICollActionEditorState {
  content?: string;
}

export default class RichTextEditor extends React.Component<ICollActionEditorProps, ICollActionEditorState> {
  constructor() {
    super();
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
    this.setState({ content: value });

    const input = document.getElementById(this.props.formInputId) as HTMLInputElement;
    if (input) {
      input.value = this.state.content;
    }
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
    formInputId="Description"
    hint="E.g Reduce plastic waste and save our oceans!" />,
  document.getElementById("create-project-description")
);

renderComponentIf(
  <RichTextEditor
    formInputId="Goal"
    hint="Max 1000 characters" />,
  document.getElementById("create-project-goal")
);

renderComponentIf(
  <RichTextEditor
    formInputId="CreatorComments"
    hint="E.g Background, process, FAQs, about the initiator" />,
  document.getElementById("create-project-creatorcomments")
);
