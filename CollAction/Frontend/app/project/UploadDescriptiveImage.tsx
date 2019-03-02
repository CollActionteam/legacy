import * as React from "react";
import UploadImage, { IUploadImageState } from "./UploadImage";
import DropZone from "react-dropzone";
import renderComponentIf from "../global/renderComponentIf";

export interface IUploadDescriptiveImageProps {
    descriptionContainer: HTMLElement;
    descriptionField: HTMLInputElement;
}

export default class UploadDescriptiveImage extends UploadImage<IUploadDescriptiveImageProps, IUploadImageState> {
    constructor(props: any) {
        super(props);

        this.state = this.createInitialState();

        this.onDrop = this.onDrop.bind(this);
        this.onRejected = this.onRejected.bind(this);
        this.rejectImage = this.rejectImage.bind(this);
        this.createSrcImage = this.createSrcImage.bind(this);
        this.getFileInputElement = this.getFileInputElement.bind(this);
    }

    onDrop(accepted: File[], rejected: File[], event: any) {
        this.loadImage(accepted, rejected, event);
    }

    onRejected() {
        this.rejectImage();
    }

    createSrcImage(): string {
        return this.state.image as string;
    }

    getFileInputElement(): HTMLInputElement {
        return document.getElementsByName("DescriptiveImageUpload")[0] as HTMLInputElement;
    }

    componentDidMount() {
        this.setDescriptionVisibility();
    }

    componentDidUpdate() {
        this.setDescriptionVisibility();
    }

    componentWillUpdate() {
        this.setDescriptionVisibility();
    }

    private setDescriptionVisibility() {
        if (this.state.preview === true) {
            this.props.descriptionContainer.classList.remove("hidden");
        }
        else {
            this.props.descriptionContainer.classList.add("hidden");
            this.props.descriptionField.value = null;
        }
    }

    renderImageControl() {
        return (
            <div className="image-control">
                <div className="description">
                    <p>The image description will appear here</p>
                </div>
                <div className="text">
                    <img src="/images/BrowserSize.png"></img>
                    <span className="mobile">
                        <p>Rotate your device, to see if the image is suitable.</p>
                    </span>
                    <span className="desktop">
                        <p>Try and change your browser size to see if the image is suitable.</p>
                    </span>
                </div>
                <div className="buttons">
                    <input type="button" value="Remove" onClick={this.resetImage}></input>
                </div>
            </div>
        );
    }

    render() {
        return (
            <div id="project-image">
                <div className="col-xs-12 col-md-8 col-md-offset-4" style={{display: this.state.preview ? "none" : "block"}}>
                    <DropZone
                        name="DescriptiveImageUpload"
                        accept="image/jpeg, image/png, image/gif, image/bmp"
                        maxSize={1024000}
                        multiple={false}
                        onDrop={this.onDrop}
                        onDropRejected={this.onRejected}>
                        {({getRootProps, getInputProps, isDragReject, open}) => {
                            return (
                                <div  {...getRootProps()}
                                    className={ (isDragReject ? "field-validation-error " : "") + "dropzone"}
                                    onClick={() => open()}>
                                    <h3>
                                        <span className="mobile">Tap to select descriptive image</span>
                                        <span className="desktop">Drop descriptive image here</span>
                                    </h3>
                                    <div className="instructions">
                                        <p className={ this.state.invalid ? "field-validation-error" : "hidden" }>
                                            This image is not valid. Please edit it or pick another one.
                                        </p>
                                        <p>
                                            Use jpg, png, gif or bmp. Max. 1 MB.
                                        </p>
                                    </div>
                                    <input {...getInputProps()} />
                                </div>
                            );
                        }}
                    </DropZone>
                </div>
                <div className="hidden-xs hidden-sm col-md-4" style={{display: this.state.preview ? "block" : "none"}}>
                    { this.renderImageControl() }
                </div>
                <div className="col-xs-12 col-md-8" style={{display: this.state.preview ? "block" : "none"}}>
                    <img id="preview" src={this.createSrcImage()} className="pull-right"></img>
                </div>
                <div className="col-xs-12 hidden-md hidden-lg" style={{display: this.state.preview ? "block" : "none"}}>
                    { this.renderImageControl() }
                </div>
            </div>
        );
    }
}

let container = document.getElementById("create-project-upload-descriptive-image");
renderComponentIf(
    <UploadDescriptiveImage
        descriptionContainer={ container && document.getElementById(container.dataset.descriptionDivId) }
        descriptionField={ container && document.getElementsByName(container.dataset.descriptionFieldName)[0] as HTMLInputElement } />,
    container
);
