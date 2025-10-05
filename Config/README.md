# EWIM Dynamic Threshold Configuration

This directory contains configuration files for the EWIM Dynamic Threshold System.

## Files

### dynamic_thresholds.json
Contains the current threshold configuration for all indicators. This file is automatically created and updated when you calibrate thresholds using the console interface.

### baseline_history.json
Maintains a history of the last 10 baseline captures for reference and troubleshooting.

## Console Commands

When running the EWIM application, you can use these keyboard commands:

- **C** - Start baseline capture (60 seconds)
- **S** - Stop current baseline capture early
- **A** - Apply captured baseline to calculate new thresholds
- **V** - View current thresholds
- **H** - Show baseline history
- **R** - Reset to default thresholds
- **T** - Show current indicator status
- **?** - Show help menu
- **Q** - Quit application

## Calibration Workflow

1. Run your drilling simulator in a stable, normal operating configuration
2. Press **C** to start baseline capture
3. Wait 60 seconds (or press **S** to stop early)
4. Press **A** to analyze the baseline and generate new thresholds
5. Review the calibration report and confirm to apply new thresholds
6. New thresholds are automatically saved and applied

## Threshold Calculation Methods

The system uses statistical analysis to calculate thresholds:

- **Green Max**: Mean + 1 standard deviation (normal operation range)
- **Yellow Max**: Mean + 2 standard deviations (warning threshold)
- **Red**: Beyond Yellow Max (critical threshold)

## Backup and Recovery

- Configuration files are automatically backed up before major changes
- Use **R** to reset to factory defaults if needed
- Baseline history provides audit trail of calibration activities
