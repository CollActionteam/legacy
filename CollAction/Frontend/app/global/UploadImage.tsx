import * as React from "react";
import DropZone from "react-dropzone";
import renderComponentIf from "./renderComponentIf";

interface IUploadImageProps {
}

interface IUploadImageState {
    invalid: boolean;
    preview: boolean;
}

export default class UploadImage extends React.Component<IUploadImageProps, IUploadImageState> {
    constructor(props: {}) {
        super(props);

        this.state = {
            invalid: false,
            preview: true
        };

        this.onDrop = this.onDrop.bind(this);
        this.onRejected = this.onRejected.bind(this);
        this.setBackground = this.setBackground.bind(this);
        this.removebackground = this.removebackground.bind(this);
    }

    onDrop(files: File[]) {
        if (files.length !== 1) {
            this.setState({
                invalid: true
            });
            return;
        }

        let file = files[0];
        let reader = new FileReader();
        let that = this;
        reader.onload = function() {
            that.setBackground(file.type, reader.result);
        };
        reader.onabort = this.onRejected;
        reader.onerror = this.onRejected;

        reader.readAsDataURL(file);
    }

    setBackground(type: string, image: any) {
        let background = document.getElementById("project-background");
        background.style.backgroundImage = "url(data:" + type + ";" + image + ")";
        this.setState({
            invalid: false,
            preview: true
        });
    }

    removebackground() {
        let background = document.getElementById("project-background");
        background.style.backgroundImage = null;
        this.setState({
            preview: false
        });
    }

    onRejected() {
        this.setState({
            invalid: true
        });
    }

    render() {
        if (!this.state.preview) {
            return (
                <DropZone
                    className="dropzone"
                    accept="image/jpeg, image/png, image/gif, image/bmp"
                    maxSize={1024000}
                    multiple={false}
                    onDrop={this.onDrop}
                    onDropRejected={this.onRejected}
                    rejectClassName="invalid">
                    <h3>Upload banner image</h3>
                    <p className={this.state.invalid ? "invalid" : ""}>Drop or tap. Use jpg, png, gif or bmp. Max. 1 MB</p>
                </DropZone>
            );
        }
        else {
            return (
                <div className="preview">
                    <p>Try and change your browser size, or rotate your device, to see if the image is suitable as banner</p>
                    <a className="btn center" href="#" onClick={this.removebackground}>Remove banner</a>
                </div>
            );
        }
    };
}

renderComponentIf(
    <UploadImage></UploadImage>,
    document.getElementById("create-project-upload-banner-image")
);
