import * as React from "react";
import { FormikProps } from "formik";

export interface IUploadImageProps {
  formik: FormikProps<any>;
}

export interface IUploadImageState {
  invalid: boolean;
  preview: boolean;
  image: any;
  ie11: boolean;
}

export default abstract class UploadImage<
  P extends IUploadImageProps,
  S extends IUploadImageState
> extends React.Component<P, S> {
  constructor(props: any) {
    super(props);

    this.loadImage = this.loadImage.bind(this);
    this.rejectImage = this.rejectImage.bind(this);
    this.resetImage = this.resetImage.bind(this);
  }

  abstract render();

  protected createInitialState(): IUploadImageState {
    return {
      invalid: false,
      preview: false,
      image: null,
      ie11:
        navigator.userAgent.indexOf("MSIE") !== -1 ||
        navigator.appVersion.indexOf("Trident/") > 0,
    };
  }

  protected loadImage(accepted: File[], onLoaded) {
    if (this.state.preview) {
      // On Safari and Chrome, setBannerImageUploadInput triggers this function again.
      // On Firefox it does not... so, if preview is already in progress, we can stop.
      return;
    }

    if (accepted.length !== 1) {
      this.setState({ invalid: true });
      return;
    }

    const that = this;
    this.setState({ invalid: false, preview: true });

    const reader = new FileReader();
    const file = accepted[0];
    reader.onload = () => {
      that.setState({ image: reader.result });
      onLoaded();
    };
    reader.onabort = this.rejectImage;
    reader.onerror = this.rejectImage;

    reader.readAsDataURL(file);
  }

  protected rejectImage() {
    this.setState({ invalid: true, preview: false });
  }
  protected resetImage() {
    this.setState({ preview: false, image: null });
  }
}
