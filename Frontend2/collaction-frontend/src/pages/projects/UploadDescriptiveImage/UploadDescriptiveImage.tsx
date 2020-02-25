import * as React from "react";
import UploadImage, {
  IUploadImageState,
  IUploadImageProps,
} from "../../../components/UploadImage";
import { TertiaryButton } from "../../../components/Button/Button";
import DropZone from "react-dropzone";

import BrowserSizeImage from "../../../assets/BrowserSize.png";
import styles from "../Create/Create.module.scss";

import {
  Grid,
  Container,
  FormControl,
  TextField,
  Hidden,
} from "@material-ui/core";

interface IUploadDescriptiveImageState extends IUploadImageState {
  description: string;
}

export default class UploadDescriptiveImage extends UploadImage<
  IUploadImageProps,
  IUploadDescriptiveImageState
> {
  constructor(props: {}) {
    super(props);

    this.state = {
      ...super.createInitialState(),
      description: "",
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

  renderPreviewControl() {
    return (
      <>
        {this.state.preview && (
          <div className={styles.descriptiveImageInstruction}>
            <p className={styles.imageDescription}>
              {this.state.description !== ""
                ? this.state.description
                : "The image description will appear here"}
            </p>
            <div className={styles.imagePreview}>
              <img src={BrowserSizeImage}></img>
              <div className={styles.dropzoneInstruction}>
                Resize your browser or rotate your device to see if the image
                suitable.
              </div>
              <TertiaryButton onClick={this.resetImage}>
                Remove banner
              </TertiaryButton>
            </div>
          </div>
        )}
      </>
    );
  }

  setImageDescription(event: any) {
    const description = event.target.value;
    this.setState({
      description,
    });

    this.props.formik.setFieldValue("imageDescription", description);
  }

  render() {
    return (
      <>
        <Grid container>
          <Grid item xs={12} md={5}>
            <Container>
              <Hidden smDown>{this.renderPreviewControl()}</Hidden>
            </Container>
          </Grid>

          <Grid item xs={12} md={7}>
            <Container>
              <div className={styles.uploadDescriptiveImage}>
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
                          <h3>Drop a image / tap to select</h3>
                          <div className={styles.dropzoneInstruction}>
                            {this.state.invalid && (
                              <div className={styles.error}>
                                This image is not valid. Please edit it or pick
                                another one.
                              </div>
                            )}
                            <div
                              className={this.state.invalid ? styles.error : ""}
                            >
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
                  <img
                    className={styles.descriptiveImagePreview}
                    src={this.state.image}
                  ></img>
                )}
              </div>
            </Container>
          </Grid>
        </Grid>
        {this.state.preview && (
          <Grid container>
            <Grid item xs={12} md={5}></Grid>
            <Grid item xs={12} md={7}>
              <Container className={styles.form}>
                <Hidden mdUp>{this.renderPreviewControl()}</Hidden>
                <FormControl>
                  <TextField
                    label="Image description"
                    helperText="A caption for your image"
                    onChange={e => this.setImageDescription(e)}
                  ></TextField>
                </FormControl>
              </Container>
            </Grid>
          </Grid>
        )}
      </>
    );
  }
}
