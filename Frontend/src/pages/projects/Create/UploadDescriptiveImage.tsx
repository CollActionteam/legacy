import * as React from "react";
import DropZone from "react-dropzone";
import UploadImage, { IUploadImageProps } from "../../../components/UploadImage";
import styles from "./UploadDescriptiveImage.module.scss";
import { Field } from "formik";
import { TextField } from "formik-material-ui";



export default class UploadDescriptiveImage extends UploadImage<
  IUploadImageProps, any
> {
  constructor(props: {}) {
    super(props);

    this.state = {
      ...super.createInitialState()
    };

    this.onDrop = this.onDrop.bind(this);
    this.onRejected = this.onRejected.bind(this);
    this.createCssImage = this.createCssImage.bind(this);
    this.createSrcImage = this.createSrcImage.bind(this);
  }

  onDrop(accepted: File[], _rejected: File[], _event: any) {
    this.loadImage(accepted, (file: File) =>
      this.props.formik.setFieldValue("image", file)
    );
  }

  onRejected() {
    this.rejectImage();
  }

  createCssImage(): React.CSSProperties {
    if (this.state.image === null) {
      return {};
    }

    return {
      backgroundImage: `url(${this.state.image}`,
      backgroundPosition: "center",
      backgroundRepeat: "no-repeat",
      backgroundSize: "cover",
    };
  }

  uploadCardStyle() {
    if (this.state.image === null) {
      return {};
    }

    return {
      opacity: 0.8,
    };
  }

  createSrcImage(): string {
    return this.state.image as string;
  }

  render() {
    const classNames = `${this.props.className} ${styles.upload}`;
    return (
      <div className={classNames}>
        {!this.state.preview && (
          <DropZone
            accept="image/jpeg, image/png, image/gif, image/bmp"
            maxSize={1024000}
            multiple={false}
            onDrop={this.onDrop}
            onDropRejected={this.onRejected}
          >
            {({ getRootProps, getInputProps, open }: any) => {
              return (
                <div
                  {...getRootProps()}
                  className={styles.dropzone}
                  onClick={() => open()}
                >
                  <h3>Drop a descriptive image or tap to select one</h3>
                  <div className={styles.dropzoneInstruction}>
                    {this.state.invalid && (
                      <div className={styles.error}>
                        This image is not valid. Please edit it or pick
                        another one.
                      </div>
                    )}
                    <div className={this.state.invalid ? styles.error : ""}>
                      Use jpg, png, gif or bmp. Max. 1 MB.
                    </div>
                  </div>
                  <input {...getInputProps()}></input>
                </div>
              );
            }}
          </DropZone>
        )}

        {this.state.preview && (
          <React.Fragment>
            <img
              className={styles.descriptiveImagePreview}
              src={this.state.image}
              alt="Preview"
            >              
            </img>
            <Field
              name="imageDescription"
              label="Image description"
              component={TextField}
              className={styles.formRow}
              helperText="Provide a short description of this image"
              fullWidth
            >
            </Field>
          </React.Fragment>
        )}
      </div>
    );
  }
}
