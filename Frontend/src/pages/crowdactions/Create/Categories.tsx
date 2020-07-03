import React from 'react';
import { FormControl, InputLabel, MenuItem, FormHelperText } from '@material-ui/core';
import { Select } from 'formik-material-ui';
import styles from './Create.module.scss';
import { Field } from 'formik';

import Utils from '../../../utils';
import { useSettings } from '../../../providers/SettingsProvider';

const Categories = (formik: any) => {
  const { categories } = useSettings();

  return (
    <>
      <FormControl fullWidth className={styles.formRow}>
        <InputLabel htmlFor="category">Category</InputLabel>
        <Field 
          name="category" 
          component={Select} 
          error={formik.props.touched.category && Boolean(formik.props.errors.category)}
        >
          {categories.map((category: string) => (
            <MenuItem key={category} value={category}>
              { Utils.formatCategory(category) }
            </MenuItem>
          ))}
        </Field>
        <FormHelperText error={formik.props.touched.category && Boolean(formik.props.errors.category)}>
          {formik.props.errors.category}
        </FormHelperText>
        <FormHelperText hidden={formik.props.touched.category && Boolean(formik.props.errors.category)}>
          Choose the category that most closely aligns with your crowdaction
        </FormHelperText>
      </FormControl>
      <FormControl fullWidth className={styles.formRow}>
        <InputLabel htmlFor="secondCategory">Second Category</InputLabel>
        <Field 
          name="secondCategory" 
          component={Select} 
          error={formik.props.touched.secondCategory && Boolean(formik.props.errors.secondCategory)}
        >
          {categories.map((category: string) => (
            <MenuItem key={category} value={category}>
              { Utils.formatCategory(category) }
            </MenuItem>
          ))}
        </Field>
        <FormHelperText error={formik.props.touched.secondCategory && Boolean(formik.props.errors.secondCategory)}>
          {formik.props.errors.secondCategory}
        </FormHelperText>
        <FormHelperText hidden={formik.props.touched.secondCategory && Boolean(formik.props.errors.secondCategory)}>
          Choose an optional second category that also aligns with your crowdaction
        </FormHelperText>
      </FormControl>
    </>
  );
}

export default Categories;
