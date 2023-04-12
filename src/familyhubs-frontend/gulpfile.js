"use strict";

const gulp = require("gulp"),
    rename = require('gulp-rename'),
    fs = require('fs');

gulp.task('copy-wwwroot', function () {
    return gulp.src('wwwroot/**/*')
        .pipe(gulp.dest('../../wwwroot'));
});

function copyPackageJsToWwwroot(packageName, srcFilename) {
    // Read the package.json file to get the package version
    const packageJson = JSON.parse(fs.readFileSync(`../${packageName}/package.json`));
    const packageVersion = packageJson.version;

    // Set the destination file name
    const destFileName = `${packageName}-${packageVersion}.min.js`;

    // Copy and rename the file
    return gulp.src(`../${packageName}/${srcFilename}`)
        .pipe(rename(destFileName))
        .pipe(gulp.dest('../../wwwroot/js'));
}

gulp.task('copy-govuk-frontend-js', function () {
    return copyPackageJsToWwwroot('govuk-frontend', 'govuk/all.js');
});

gulp.task('copy-familyhubs-frontend-js', function () {
    return copyPackageJsToWwwroot('familyhubs-frontend', 'all.min.js');
});

gulp.task('populate-wwwroot', gulp.series('copy-wwwroot', 'copy-familyhubs-frontend-js'));

//todo: delegate from consumer gulp to this gulp?