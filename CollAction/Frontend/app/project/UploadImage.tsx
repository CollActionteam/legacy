import * as React from "react";

export interface IUploadImageProps {
    descriptionDivId?: string;
    descriptionFieldName?: string;
}

export interface IUploadImageState {
    invalid: boolean;
    preview: boolean;

    image: any;
}

export default abstract class UploadImage extends React.Component<IUploadImageProps, IUploadImageState> {
    constructor(props: {}) {
        super(props);

        this.state = {
            invalid: false,
            preview: false,
            image: null
        };

        this.loadImage = this.loadImage.bind(this);
        this.rejectImage = this.rejectImage.bind(this);
        this.resetImage = this.resetImage.bind(this);
    }

    protected loadImage(accepted: File[], rejected: File[], event: any) {
        if (this.state.preview) {
            // On Safari and Chrome, setBannerImageUploadInput triggers this function again.
            // On Firefox it does not... so, if preview is already in progress, we can stop.
            return;
        }

        if (accepted.length !== 1) {
            this.setState({ invalid: true });
            return;
        }

        let that = this;
        this.setState({ invalid: false, preview: true});

        if (event.dataTransfer) {
            // onDrop was triggered from drag and drop event.
            // Add the files from the event to the BannerImageUpload input element.
            let files = event.dataTransfer.files;
            setTimeout(function() {
                // Schedule this for the next event loop.
                // Otherwise, on Chrome and Safari it will trigger onDrop in the same event loop and we won't have the correct state.
                let inputElement = that.getFileInputElement();
                inputElement.files = files;
            });
        }

        // Note: when the user clicked or tapped on the dropzone and selected a file,
        // then the files will already be in the file input element.

        let reader = new FileReader();
        let file = accepted[0];
        reader.onload = function() {
            let url = that.createImage(file.type, reader.result);
            that.setState({ image: url });
        };
        reader.onabort = this.rejectImage;
        reader.onerror = this.rejectImage;

        reader.readAsDataURL(file);
    }

    abstract getFileInputElement(): HTMLInputElement;

    abstract createImage(type: string, image: any): any;

    protected rejectImage() {
        this.setState({ invalid: true, preview: false });
    }

    protected resetImage() {
        let inputElement = this.getFileInputElement();
        inputElement.value = "";

        if (this.props.descriptionFieldName) {
            let fields = document.getElementsByName(this.props.descriptionFieldName);
            for (let i = 0; i < fields.length; i++) {
                let input = fields.item(i) as HTMLInputElement;
                input.value = null;
            }
        }

        this.setState({ preview: false, image: null });
    }

    componentDidMount() {
        this.setDescriptionVisibility();
    }

    componentDidUpdate() {
        this.setDescriptionVisibility();
    }

    setDescriptionVisibility() {
        if (this.props.descriptionDivId) {
            if (this.state.preview === true) {
                document.getElementById(this.props.descriptionDivId).classList.remove("hidden");
            }
            else if (this.state.preview === false) {
                document.getElementById(this.props.descriptionDivId).classList.add("hidden");
            }
        }
    }

    abstract render();
}
