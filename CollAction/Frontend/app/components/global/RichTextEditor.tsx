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
  maxLength: number | null;
  currentLength: number;
}

export default class RichTextEditor extends React.Component<ICollActionEditorProps, ICollActionEditorState> {

  private HREF_REGEX = /href=\"(?!http:\/\/|https:\/\/)([^\"]*)(\")/ig;

  constructor(props: ICollActionEditorProps) {
    super(props);

    this.state = {
      content: "",
      maxLength: null,
      currentLength: 0
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
    if (!input) {
      return;
    }

    this.setState({
      content: input.value,
      currentLength: input.value.length
     });

    if (input.dataset.valLengthMax) {
      this.setState({
        maxLength: parseInt(input.dataset.valLengthMax),
      });
    }
  }

  handleChange(value) {
    value = this.fixLinks(value);

    this.setState({
      content: value,
      currentLength: value.length
    });

    const input = document.getElementById(this.props.formInputId) as HTMLInputElement;
    if (input) {
      input.value = this.state.content;

      // Manually dispatch change event for the hidden input field to trigger validation
      let event = new Event("change");
      input.dispatchEvent(event);
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
      <div>
        { this.state.maxLength && <p className="richtext-counter">{ this.state.currentLength.toLocaleString() } / {this.state.maxLength.toLocaleString() } </p> }
        <ReactQuill
          theme="snow"
          value={ this.state.content }
          placeholder={ this.props.hint }
          modules={ this.modules }
          formats={ this.formats }
          onChange={ this.handleChange } />
      </div>
    );
  }
}

renderComponentIf(
  <RichTextEditor
    formInputId="Message"
    hint="Hi {firstname}!, Here is our project update..." />,
  document.getElementById("project-email-message")
);

renderComponentIf(
  <RichTextEditor
    formInputId="Description"
    hint="E.g Reduce plastic waste and save our oceans!" />,
  document.getElementById("create-project-description")
);

renderComponentIf(
  <RichTextEditor
    formInputId="Goal"
    hint="What is the problem you are trying to solve?" />,
  document.getElementById("create-project-goal")
);

renderComponentIf(
  <RichTextEditor
    formInputId="CreatorComments"
    hint="E.g Background, process, FAQs, about the initiator" />,
  document.getElementById("create-project-creatorcomments")
);
